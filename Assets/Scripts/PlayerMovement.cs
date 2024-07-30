using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpVelocity = 12f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float airControlFactor = 0.5f;

    Vector2 moveInput;
    Animator animator;
    Rigidbody2D rb;
    float gravityScaleCache;

    Collider2D playerCollider;
    private bool isClimbing;
    private float originalAnimatorSpeed;


    // Update is called once per frame

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        gravityScaleCache = rb.gravityScale;
    }

    void Update()
    {
        Climb();
        Run();
    }

    private void Climb()
    {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Ladders")))
        {
            isClimbing = true;
            rb.gravityScale = 0;
            Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
            
            animator.SetBool("IsClimbing", true);
            if (MathF.Abs(moveInput.y) < Mathf.Epsilon) 
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
        bool isGrounded = IsGrounded();
        //check if the player is grounded
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }
    }

    private void Run()
    {

        float horizontalInput = moveInput.x;

        bool isGrounded = IsGrounded();
        if (!isGrounded)
        {
            horizontalInput *= airControlFactor;
        }

        rb.AddForce(new Vector2(horizontalInput * runSpeed - rb.velocity.x, 0), ForceMode2D.Impulse); 
        //velocity = new Vector2(horizontalInput * runSpeed, rb.velocity.y);

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
        // Restore the original speed
        animator.speed = originalAnimatorSpeed;
    }

    private bool IsGrounded()
    {
        return playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
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


}
