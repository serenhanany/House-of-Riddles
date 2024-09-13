using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;
using static System.Net.WebRequestMethods;
using System;



public class Practice : MonoBehaviour
{
    public TMP_Text playerNameText;
    //public TMP_Text playerScoreText;
    public TMP_Text playerLevelText;
    public TMP_Text timerText;  // Renamed Time to timerText to avoid conflict
    public Button StartPlay;
    public Button EndGame;
    private float startTime;
    public int minutes;
    public int seconds;
    public int minutes1;
    public int seconds1;
    private bool isGameRunning = true;  // Added initialization for isGameRunning
    
    void Start()
    {
        startTime = Time.time;
        if (GameData.Instance == null)
        {
            GameObject gameTimeManagerObject = new GameObject("GameTimeManager");
            gameTimeManagerObject.AddComponent<GameData>();
        }
        // Check if PlayerData is not null and initialized
        if (PlayerData.Instance != null)
        {
            // Update UI with the player's data
            UpdatePlayerInfoUI();
        }
        else
        {
            Debug.LogError("PlayerData.Instance is null. Ensure PlayerData is initialized.");
        }

        if (EndGame != null)
        {
            EndGame.onClick.AddListener(information);
        }

        if (StartPlay != null)
        {
            StartPlay.onClick.AddListener(Buildings2);
        }
    }
    public void information()
    {
        StopTimer();
        SceneManager.LoadScene("information");
    }

    public void Buildings2()
    {
        SceneManager.LoadScene("Buildings 2");
    }

    // This method updates the player's info on the screen
    void UpdatePlayerInfoUI()
    {
        // Display the player's name, score, and level on the screen
        playerNameText.text = $"Name: {PlayerData.Instance.playerName}";
        //playerScoreText.text = $"Score: {PlayerData.Instance.Score}";
        playerLevelText.text = $"Level: {PlayerData.Instance.playerLevel}";
    }

    void Update()
    {
        if (isGameRunning)
        {
            // Check if timerText is assigned before trying to update it
            if (timerText != null)
            {
                // Calculate how much time has passed since the game started
                float timeSinceStart = Time.time - startTime;

                // Convert time to minutes and seconds
                minutes = Mathf.FloorToInt(timeSinceStart / 60F);
                  seconds = Mathf.FloorToInt(timeSinceStart % 60F);

              // Display the timer in the format MM:SS
                timerText.text = string.Format("Time:{0:00}:{1:00}", minutes, seconds);
                if (minutes >= 5)
                {
                    information();  // Stop the timer when it hits 1 minute
                }
            }
            else
            {
                Debug.LogError("timerText is not assigned in the Inspector.");
            }
        }
    }


    // Optional: Add this method to stop the timer
    public void StopTimer()
    {
        float timeSinceStart1 = Time.time - startTime;
        minutes1 = Mathf.FloorToInt(timeSinceStart1 / 60F);
        seconds1 = Mathf.FloorToInt(timeSinceStart1 % 60F);
        isGameRunning = false;

        // Save the time to GameData
        if (GameData.Instance != null)
        {
            GameData.Instance.SetGameTime(minutes1, seconds1);
        }
    }

}
