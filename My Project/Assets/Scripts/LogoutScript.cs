using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogoutScript : MonoBehaviour
{
    public Button logoutButton;
    //public TMP_Text messageText;

    void Start()
    {
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(LogoutUser);
        }
    }

    public void LogoutUser()
    {
        // Clear player data
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.ClearPlayerData();
            Debug.Log("User logged out successfully.");
            //messageText.text = "You have been logged out.";

            // Redirect to the login scene
            SceneManager.LoadScene("Login");
        }
        else
        {
            Debug.LogError("PlayerData.Instance is null. Ensure PlayerData is initialized.");
        }
    }
}
