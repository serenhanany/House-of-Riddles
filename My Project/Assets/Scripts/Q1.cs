using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class Q1 : MonoBehaviour
{

    public Button YesButton;
    public Button NoButton;
    // Start is called before the first frame update
    void Start()
    {
        if (YesButton != null)
        {
            YesButton.onClick.AddListener(PracticeScene);
        }
        if (NoButton != null)
        {
            NoButton.onClick.AddListener(LoginScene);
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
}
