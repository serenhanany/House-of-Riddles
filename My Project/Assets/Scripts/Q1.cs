using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class Q1 : MonoBehaviour
{
    // Public fields to reference the UI buttons in the scene
    public Button YesButton;
    public Button NoButton;
    // Start is called before the first frame update
    void Start()
    {
        // Add listener to YesButton if it's assigned in the Inspector
        if (YesButton != null)
        {
            YesButton.onClick.AddListener(PracticeScene);
        }
        // Add listener to NoButton if it's assigned in the Inspector
        if (NoButton != null)
        {
            NoButton.onClick.AddListener(LoginScene);
        }
    }
    // Method to load the "Buildings 2" scene
    public void LoginScene()
    {
        SceneManager.LoadScene("Buildings 2");
    }
    // Method to load the "Buildings" scene
    public void PracticeScene()
    {
        SceneManager.LoadScene("Buildings");
    }
}
