using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using Mirror;


public class RegisterScript : MonoBehaviour
{
    public TMP_InputField fullnameField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField ageField;
    public TMP_InputField studyField;
    public Button registerButton;
    public TMP_Text messageText;
    public Button HomePageButton;

    void Start()
    {
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

        string username = usernameField.text;
        string password = passwordField.text;
        string age = ageField.text;
        string study = studyField.text;
        string fullname = fullnameField.text;

        // Check if any field is empty
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(age) || string.IsNullOrEmpty(study))
        {
            // Show error message
            messageText.text = "Please fill in all fields.";
            yield break; // Exit coroutine
        }

        // Convert age string to integer
        int age1;
        if (!int.TryParse(age, out age1))
        {
            // Show error message if age is not a valid integer
            messageText.text = "Please enter a valid age.";
            yield break; // Exit coroutine
        }

        // Create the user object
        UserModel user = new UserModel { Username = username, Password = password, Fullname=fullname,Age=age,Study=study };

        // Convert the user object to JSON
        string json = JsonUtility.ToJson(user);

        // Set up the UnityWebRequest
        UnityWebRequest request = new UnityWebRequest("https://LocalHost:7096/api/users/register", "POST");
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
    public string Age;
    public string Study;
}
