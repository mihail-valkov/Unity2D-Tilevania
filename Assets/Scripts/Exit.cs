using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    bool changedLevel = false;
    //Exit level on player collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!changedLevel && collision.CompareTag("Player"))
        {
            //maje the player inactive
            collision.gameObject.SetActive(false);
            //prevent multiple level changes from multiple player colliders
            changedLevel = true;
            GameManager.Instance.NextLevel();
        }
    }
}
