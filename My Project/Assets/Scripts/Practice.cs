using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;
using static System.Net.WebRequestMethods;
public class Practice : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerScoreText;
    public TMP_Text playerLevelText;
    public Button StartPlay;
    void Start()
    {
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
        if (StartPlay != null)
        {
            StartPlay.onClick.AddListener(Buildings2);
        }
    }
    public void Buildings2()
    {
        SceneManager.LoadScene("Buildings 2");
    }
    // This method updates the player's info on the screen
    void UpdatePlayerInfoUI()
    {
        // Display the player's name, score, and level on the screen
        playerNameText.text = $"Name:: {PlayerData.Instance.playerName}";
        playerScoreText.text = $"Score: {PlayerData.Instance.Score}";
        playerLevelText.text = $"Level: {PlayerData.Instance.playerLevel}";
    }
}
