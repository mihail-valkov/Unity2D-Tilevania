using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    void Update()
    {
        //destroy the bullet after 3 seconds in case it doesn't hit anything
        Destroy(gameObject, 3);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            //set enemy IsAlive
            IEnemy enemy = other.gameObject.GetComponent<IEnemy>();
            enemy.Die();
        }
            //destroy the bullet after hitting anything
        Destroy(gameObject, 0.05f);
    }
}
