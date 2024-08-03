using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IEnemy
{
    public bool IsDead 
    { 
        get; private set;
    }

    public virtual void Die()
    {
        IsDead = true;

        //disable the colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        //disable the rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //rb.bodyType = RigidbodyType2D.Kinematic;

        //throw the enemy up
        rb.velocity = new Vector2(0, 5f);

        //destroy the game object after 3 seconds
        Destroy(gameObject, 3);
    }
}
