using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int maxLives = 5;

    //textmesh pro labels for lives and coins
    [SerializeField] TMPro.TextMeshProUGUI livesLabel;
    [SerializeField] TMPro.TextMeshProUGUI coinsLabel; 

    //singleton
    public static GameManager Instance { get; private set; }

    //level index
    [SerializeField] int startLevelIndex = 0;

    int levelIndex = 0;

    void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
            if (startLevelIndex > 0)
            {
                levelIndex = startLevelIndex;
                SceneManager.LoadScene(startLevelIndex);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //initialize lives and coins
        PlayerPrefs.SetInt("Lives", maxLives);
        
        PlayerPrefs.SetInt("Coins", 0);

        //update lives and coins labels
        livesLabel.text = $"{PlayerPrefs.GetInt("Lives", maxLives)}";
        coinsLabel.text = $"{PlayerPrefs.GetInt("Coins", 0)}";
    }

    //Load next level	
    public void NextLevel()
    {
        levelIndex++;
        if (levelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            levelIndex = 0;
        }

        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        //find the ScreenPersist objectg and destroy it
        ScreenPersist screenPersist = FindObjectOfType<ScreenPersist>();
        if (screenPersist != null)
        {
            screenPersist.ResetForNextLevel();
        }

        SceneManager.LoadScene(levelIndex);
    }

    public void CollectCoin()
    {
        //update coins count and label
        int coins = PlayerPrefs.GetInt("Coins", 0);
        coins++;
        PlayerPrefs.SetInt("Coins", coins);
        coinsLabel.text = coins.ToString();
    }

    public void LoseLife()
    {
        //update lives count and label
        int lives = PlayerPrefs.GetInt("Lives", maxLives);
        lives--;
        PlayerPrefs.SetInt("Lives", lives);
        livesLabel.text = lives.ToString();

        //check if player has no lives left
        if (lives <= 0)
        {
            //reset lives and coins
            PlayerPrefs.SetInt("Lives", maxLives);
            PlayerPrefs.SetInt("Coins", 0);
            livesLabel.text = maxLives.ToString();
            coinsLabel.text = "0";

            //load first level
            levelIndex = 0;
            LoadCurrentLevel();
        }
        else
        {
            //restart the level
            StartCoroutine(RestartLevel());
        }
        
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
