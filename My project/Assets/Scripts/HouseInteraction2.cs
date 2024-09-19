using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HouseInteraction2 : MonoBehaviour
{
    // Public fields to reference UI elements in the scene
    public GameObject QuestionPanel2;  // Reference to the panel in the scene
    public TMP_Text CountOfHousesText;
    
    private bool isLocked = true;

    private void Start()
    {
        // Ensure the cursor is visible and not locked when the scene starts
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
    }

    private void OnMouseDown()
    {
        // Check if the house is locked
        if (isLocked)
        {
            if (QuestionPanel2 != null)
            {
                QuestionPanel2.SetActive(true);
                if (CountOfHousesText != null)
                {
                    CountOfHousesText.text = "";
                }
                else
                {
                    Debug.LogError("CountOfHousesText is not assigned in the Inspector!");
                }
            }
            else
            {
                Debug.LogError("QuestionPanel2 is not assigned in the Inspector!");
            }

            Debug.Log("House clicked: " + gameObject.name);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    // Method to unlock the house, changing its state to unlocked
    public void UnlockHouse()
    {
        isLocked = false;
        Debug.Log(gameObject.name + " has been unlocked!");
    }
    // Method to close the question panel
    public void ClosePanel()
    {
        if (QuestionPanel2 != null)
        {
            QuestionPanel2.SetActive(false);
        }
        else
        {
            Debug.LogError("QuestionPanel2 is not assigned in the Inspector!");
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
