using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class LearningScreen : MonoBehaviour
{
    public Button LoginButton;
    public Button HomePageButton;
    // Start is called before the first frame update
    void Start()
    {
        if (HomePageButton != null)
        {
            HomePageButton.onClick.AddListener(HomePageScene);
        }
        if (LoginButton != null)
        {
            LoginButton.onClick.AddListener(LoginScene);
        }
    }
    public void HomePageScene()
    {
        SceneManager.LoadScene("HomePage");
    }
    public void LoginScene()
    {
        SceneManager.LoadScene("Login");
    }
    // Update is called once per frame

}
