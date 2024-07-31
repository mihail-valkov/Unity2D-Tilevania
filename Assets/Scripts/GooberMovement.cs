using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooberMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float jumpIntervalSeconds = 2f;

    float prevJumpTime = 0f;
    float randomJumpTimeOffset;
    Rigidbody2D rb;

    void Awake()
    {
        prevJumpTime = Random.Range(0f, jumpIntervalSeconds);
        //find the rigid body 2d
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //jump every 3 seconds towards the player
        if (Time.time - prevJumpTime > jumpIntervalSeconds)
        {
            JumpTowardsPlayer();
            prevJumpTime = Time.time;
        }
    }

    private void JumpTowardsPlayer()
    {
        Vector3 playerPos = GameObject.Find("Ginger (Player)").transform.position;
        Vector3 direction = playerPos - transform.position;
        direction.Normalize();
        //jump up and towards the player
        rb.AddForce(direction * moveSpeed, ForceMode2D.Impulse);
        //stop the walker from moving in the y direction
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        //Flip the sprite to face to the player
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        //use Goober animator to trigger Jump
        GetComponent<Animator>().SetTrigger("Jump");
    }
}
