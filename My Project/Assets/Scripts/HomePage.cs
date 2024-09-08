using UnityEngine;
using UnityEngine.UI;
using TMPro; // for TextMeshPro
using UnityEngine.SceneManagement;

public class HomePage : MonoBehaviour
{
    public Button PlayButton;
    public Button PracticeButton;
    public Button Help;       // Updated to match your naming convention
    public Button createAccountButton;
    public TextMeshProUGUI helpText; // Reference to the TextMeshProUGUI element
    private bool isHelpTextVisible = false;

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
        if (Help != null)
        {
            Help.onClick.AddListener(ToggleHelpText);
        }
        if (createAccountButton != null)
        {
            createAccountButton.onClick.AddListener(createAccountScene);
        }

        // Initially hide the help text
        if (helpText != null)
        {
            helpText.gameObject.SetActive(false);
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

    // Toggle the visibility of the help text
    public void ToggleHelpText()
    {
        if (helpText != null)
        {
            isHelpTextVisible = !isHelpTextVisible;
            helpText.gameObject.SetActive(isHelpTextVisible);
        }
    }

    public void createAccountScene()
    {
        SceneManager.LoadScene("Register");
    }
}
