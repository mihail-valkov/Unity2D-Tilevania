using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPersist : MonoBehaviour
{
    //singleton
    public static ScreenPersist Instance { get; private set; }

    public void ResetForNextLevel()
    {
        Instance = null;
        Destroy(this.gameObject);
    }

    private void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
