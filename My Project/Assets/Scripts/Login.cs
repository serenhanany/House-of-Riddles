using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;


public class LoginScript : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public Button HomePageButton;
    public Button createAccountButton;
    public TMP_Text messageText;

    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(LoginUser()));

        if (HomePageButton != null)
        {
            HomePageButton.onClick.AddListener(HomePageScene);
        }
        if (createAccountButton != null)
        {
            createAccountButton.onClick.AddListener(createAccountScene);
        }
    }

    public void createAccountScene()
    {
        SceneManager.LoadScene("Register");
    }

    public void HomePageScene()
    {
        SceneManager.LoadScene("HomePage");
    }

    IEnumerator LoginUser()
    {
        string url = "https://localhost:7096/api/users/login";
        Debug.Log("Connecting to URL: " + url);
        messageText.text = "";

        string username = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Please fill in all fields.";
            yield break;
        }

        LoginModel login = new LoginModel { Username = username, Password = password };
        string json = JsonUtility.ToJson(login);
        Debug.Log("JSON to be sent: " + json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            messageText.text = "Login failed. Please try again.";
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                Debug.Log("Login successful!");
                messageText.text = "Login successful!";

                // Parse the server response and store the player data
                LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);

                if (response != null)
                {
                    Debug.Log($"Username: {response.Username}, UserId: {response.UserId}, Level: {response.Level}");

                    // Store the player data in PlayerData
                    if (PlayerData.Instance != null)
                    {
                        PlayerData.Instance.SetPlayerData(response.UserId, response.Username, response.Level);

                        // Navigate to the next scene
                        SceneManager.LoadScene("questiontest");
                    }
                    else
                    {
                        Debug.LogError("PlayerData.Instance is null. Ensure PlayerData is initialized.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse JSON into LoginResponse. Response was null.");
                }
            }
            else
            {
                Debug.LogError("Login failed: " + request.downloadHandler.text);
                messageText.text = "Login failed: " + request.downloadHandler.text;
            }
        }
    }
}

[System.Serializable]
public class LoginModel
{
    public string Username;
    public string Password;
}

[System.Serializable]
public class LoginResponse
{
    public int UserId;
    public string Username;
    public int Level;
    public string Message;  // If you want to handle the message as well
}

