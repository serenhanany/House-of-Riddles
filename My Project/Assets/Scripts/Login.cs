using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
            // Add a listener to the button's click event
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

        // Check if any field is empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // Show error message
            messageText.text = "Please fill in all fields.";
            yield break; // Exit coroutine
        }

        // Create the login object
        LoginModel login = new LoginModel { Username = username, Password = password };

        // Convert the login object to JSON
        string json = JsonUtility.ToJson(login);
        Debug.Log("JSON to be sent: " + json);

        // Set up the UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
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
                SceneManager.LoadScene("Game");
                // Handle successful login (e.g., navigate to the main game scene)
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
