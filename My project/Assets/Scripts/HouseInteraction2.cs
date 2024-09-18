using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseInteraction2 : MonoBehaviour
{
    public GameObject QuestionPanel2;  // Reference to the panel in the scene
    public TMP_Text CountOfHousesText;
    
    private bool isLocked = true;

    private void Start()
    {
        // Ensure the cursor is always visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
    }

    private void OnMouseDown()
    {
        if (isLocked)
        {
            if (QuestionPanel2 != null)
            {
                QuestionPanel2.SetActive(true);
                CountOfHousesText.text = "";
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

    public void UnlockHouse()
    {
        isLocked = false;
        Debug.Log(gameObject.name + " has been unlocked!");
    }

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

    // Show the hint text when the button is clicked
   /* private void ShowHint()
    {
        Debug.Log("Hint button clicked!");
        if (TextForHint != null)
        {
            TextForHint.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("TextForHint is not assigned in the Inspector!");
        }
    }*/
}
