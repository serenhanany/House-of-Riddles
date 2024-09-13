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



public class information : MonoBehaviour
{
    public Practice practice;
    public TMP_Text gameOverText;
    public TMP_Text playerNameText;
    public Button startGamerButton;
    public string time;
    public TMP_Text timerText;
   
    // Start is called before the first frame update
    void Start()
    {
        if (startGamerButton != null)
        {
            startGamerButton.onClick.AddListener(startGame);
        }
        
        if (PlayerData.Instance != null)
        {
            
            playerNameText.text = $"Thank You {PlayerData.Instance.playerName}";
            if (GameData.Instance != null)
            {
                (int minutes, int seconds) = GameData.Instance.GetGameTime();
                // Display the time on the screen
                timerText.text = string.Format("Total Time Played: {0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                Debug.LogError("GameData.Instance is null. Ensure GameData is initialized.");
            }
            //Display the Game Over message with player's data
            gameOverText.text =$"You reached Level {PlayerData.Instance.playerLevel} with a score of {PlayerData.Instance.Score}.\n" +
                                "Can you do better next time?\n";
            
        }
        else
        {
            Debug.LogError("PlayerData.Instance is null. Ensure PlayerData is initialized.");
            gameOverText.text = "Game Over! Something went wrong. Please try again.";
        }
    }


    public void startGame()
    {
        SceneManager.LoadScene("Buildings 2");
    }


}
