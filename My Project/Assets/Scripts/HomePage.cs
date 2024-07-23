using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
public class HomePage : MonoBehaviour
{
    public Button PlayButton;
    public Button PracticeButton;
    public Button HelpButton;
    public Button createAccountButton;
    // Start is called before the first frame update
    void Start()
    {  
        if (PlayButton != null)
        {
            // Add a listener to the button's click event
            PlayButton.onClick.AddListener(LoginScene);
        }
        if (PracticeButton != null)
        {
            // Add a listener to the button's click event
            PracticeButton.onClick.AddListener(PracticeScene);
        }
        if (HelpButton != null)
        {
            // Add a listener to the button's click event
            HelpButton.onClick.AddListener(HelpScene);
        }
        if (createAccountButton != null)
        {
            // Add a listener to the button's click event
            createAccountButton.onClick.AddListener(createAccountScene);
        }

    }
    public void LoginScene()
    {
        SceneManager.LoadScene("Login");
    }
    public void PracticeScene()
    {
        SceneManager.LoadScene("Practice");
    }
    public void HelpScene()
    {
       // SceneManager.LoadScene("Login");
    }
    public void createAccountScene()
    {
        SceneManager.LoadScene("Register");
    }

}
