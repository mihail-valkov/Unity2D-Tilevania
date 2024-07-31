using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector2 deathKickVelocity =  new Vector2(2f, 10f);
    [SerializeField] float jumpVelocity = 12f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float airControlFactor = 0.5f;

    Vector2 moveInput;
    Animator animator;
    Rigidbody2D rb;
    float gravityScaleCache;

    BoxCollider2D playerFeetCollider;
    CapsuleCollider2D playerBodyCollider;
    private bool isClimbing;
    private float originalAnimatorSpeed;
    bool isAlive = true;


    // Update is called once per frame

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        isAlive = true;
        gravityScaleCache = rb.gravityScale;
    }

    void Update()
    {
        if (!isAlive)
            return;

        Climb();
        Run();
    }

    private void Climb()
    {
        if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladders")))
        {
            isClimbing = true;
            rb.gravityScale = 0;
            Vector2 climbVelocity = new Vector2(moveInput.x * climbSpeed, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
            
            animator.SetBool("IsClimbing", true);
            if (MathF.Abs(moveInput.y) < Mathf.Epsilon && MathF.Abs(moveInput.x) < Mathf.Epsilon) 
            {
                PauseAnimation();
            }
            else
            {
                ResumeAnimation();
            }
        }
        else if (isClimbing)
        {
            ResumeAnimation();
            rb.gravityScale = gravityScaleCache;
            animator.SetBool("IsClimbing", false);
            isClimbing = false;
        }
    }

    private void Jump()
    {
        if (!isAlive)
            return;
        
        bool isGrounded = IsGrounded();
        //check if the player is grounded
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }
    }

    private void Run()
    {

        if (isClimbing)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        float horizontalInput = moveInput.x;
        bool isGrounded = IsGrounded();

        if (!isGrounded)
        {
            //appy air control factor depending on air drag
            horizontalInput *= airControlFactor;

            if (horizontalInput > 0f)
            {
                rb.velocity = new Vector2(MathF.Max(rb.velocity.x, horizontalInput * runSpeed), rb.velocity.y);
            }
            else if (horizontalInput < 0f)
            {
                rb.velocity = new Vector2(MathF.Min(rb.velocity.x, horizontalInput * runSpeed), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * runSpeed, rb.velocity.y);
        }

        //change the animation param
        //check if the velocity is within epsylon

        if (MathF.Abs(rb.velocity.x) < Mathf.Epsilon)
        {
            animator.SetBool("IsRunning", false);
        }
        else
        {
            //flip haracter depending on the movement dir through transform.localScale.x
            transform.localScale = new Vector2(MathF.Sign(rb.velocity.x), 1f);

            if (isGrounded)
            {
                animator.SetBool("IsRunning", true);
            }
        }
    }

    // Method to pause the animation
    public void PauseAnimation()
    {
        // Store the original speed before pausing
        if (animator.speed == 0)
            return;

        originalAnimatorSpeed = animator.speed;
        animator.speed = 0;
    }

    // Method to resume the animation
    public void ResumeAnimation()
    {
        if (originalAnimatorSpeed == 0)
            return;
        // Restore the original speed
        animator.speed = originalAnimatorSpeed;
    }

    private bool IsGrounded()
    {
        return playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Jump();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the walker collides with the wall, flip the sprite
        //check only if the boxCollider has hit the wall

        if (playerBodyCollider.IsTouching(collision.collider) && collision.gameObject.tag == "Enemy")
        {
            Die();

        }
    }
    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("IsDead");

        //Flip the player a bit in the air and then disable the colliders and camera follow
        rb.velocity = deathKickVelocity;
        playerBodyCollider.enabled = false;
        playerFeetCollider.enabled = false;

        //wait for 3 seconds and restart the level
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
