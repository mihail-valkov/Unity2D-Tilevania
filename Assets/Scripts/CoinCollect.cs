using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{    
    bool coinCollected = false;

    //Collect the coin on trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        //prevent coin collection twice

        if (!coinCollected && other.gameObject.CompareTag("Player"))
        {
            coinCollected = true;
            //Add 1 to the coin count
            GameManager.Instance.CollectCoin();
            //Play audio clip
            GetComponent<AudioSource>().Play();
            //Disable the coin sprite
            GetComponent<SpriteRenderer>().enabled = false;

            //Destroy the coin
            Destroy(gameObject, 0.5f);
        }
    }
}
