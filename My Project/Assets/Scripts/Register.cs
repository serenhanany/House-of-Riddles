using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;


public class RegisterScript : MonoBehaviour
{
    public TMP_InputField fullnameField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField emailField;
    public Button registerButton;
    public TMP_Text messageText;
    public Button HomePageButton;

    void Start()
    {
        // Check if all fields are assigned
        if (fullnameField == null || usernameField == null || passwordField == null || emailField == null || registerButton == null || messageText == null)
        {
            Debug.Log("One or more fields are not assigned in the Inspector.");
            return;
        }

        registerButton.onClick.AddListener(() => StartCoroutine(RegisterUser()));

        if (HomePageButton != null)
        {
            HomePageButton.onClick.AddListener(HomePageScene);
        }
    }

    public void HomePageScene()
    {
        SceneManager.LoadScene("HomePage");
    }

    IEnumerator RegisterUser()
    {
        string url = "https://localhost:7096/api/users/register";
        Debug.Log("Connecting to URL: " + url);

        // Check if any field is empty
        if (string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(passwordField.text) || string.IsNullOrEmpty(fullnameField.text) || string.IsNullOrEmpty(emailField.text))
        {
            // Show error message
            messageText.text = "Please fill in all fields.";
            yield break; // Exit coroutine
        }

        // Collect data from input fields
        string username = usernameField.text;
        string password = passwordField.text;
        string fullname = fullnameField.text;
        string email = emailField.text;

        // Create the user object
        UserModel user = new UserModel { Username = username, Password = password, Fullname = fullname, Email = email };

        // Convert the user object to JSON
        string json = JsonUtility.ToJson(user);

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
            messageText.text = "Registration failed. Please try again.";
        }
        else
        {
            Debug.Log("User registered successfully!");
            SceneManager.LoadScene("Login");
        }
    }
}

[System.Serializable]
public class UserModel
{
    public string Username;
    public string Password;
    public string Fullname;
    public string Email;
}
