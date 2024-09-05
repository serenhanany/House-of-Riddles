using UnityEngine;
using UnityEngine.UI;
using TMPro; // for TextMeshPro, if used
using UnityEngine.SceneManagement;

public class HomePage : MonoBehaviour
{
    public Button PlayButton;
    public Button PracticeButton;
    public Button HelpButton;
    public Button createAccountButton;
    public GameObject helpPanel; // Reference to the panel

    // Start is called before the first frame update
    void Start()
    {
        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(LoginScene);
        }
        if (PracticeButton != null)
        {
            PracticeButton.onClick.AddListener(PracticeScene);
        }
        if (HelpButton != null)
        {
            HelpButton.onClick.AddListener(ToggleHelpPanel);
        }
        if (createAccountButton != null)
        {
            createAccountButton.onClick.AddListener(createAccountScene);
        }

        // Initially hide the help panel
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
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

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }

    public void createAccountScene()
    {
        SceneManager.LoadScene("Register");
    }
}
