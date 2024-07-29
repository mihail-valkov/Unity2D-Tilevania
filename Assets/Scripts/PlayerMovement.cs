using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float jumpVelocity = 8f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float airControlFactor = 0.5f;

    Vector2 moveInput;
    Animator animator;
    Rigidbody2D rb;
    bool isGrounded;

    // Update is called once per frame

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        isGrounded = Mathf.Abs(rb.velocity.y) < 0.2f;

        Run();
    }

    private void Jump()
    {
        //check if the player is grounded
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }
    }

    private void Run()
    {
        //get player rigid body
        float horizontalInput = moveInput.x;
        if (!isGrounded)
        {
            horizontalInput *= airControlFactor;
        }

        rb.velocity = new Vector2(horizontalInput * runSpeed, rb.velocity.y);


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
