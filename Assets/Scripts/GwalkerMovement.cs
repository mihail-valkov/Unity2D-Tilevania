using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GwalkerMovement : BaseEnemy
{
    [SerializeField] private float walkSpeed = 2f;
    private Vector3 direction;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    private void Awake()
    {
        //find the rigid body 2d
        rb = GetComponent<Rigidbody2D>();
        //get the box collider 2d
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsDead)
            return;

        //move the walker to the right
        rb.velocity = new Vector2(walkSpeed, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDead)
            return;
        //if the walker collides with the wall, flip the sprite
        //check only if the boxCollider has hit the wall

        if (boxCollider.IsTouching(collision.collider) && collision.gameObject.tag == "Platforms")
        {
             //flip the sprite
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            //change direction
            walkSpeed *= -1;
        }
    }
}
