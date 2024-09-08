using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public GameObject QuestionPanel2;  // Reference to the panel to show
    private bool isLocked = true;      // Flag to check if the house is locked

    private void OnMouseDown()
    {
        // Debug log to see if the click is being registered
        Debug.Log("Door clicked: " + gameObject.name);

        // Check if the house is locked
        if (isLocked)
        {
            // Show the panel with the question if the door is clicked
            QuestionPanel2.SetActive(true);
            Debug.Log("Panel shown for: " + gameObject.name);
        }
        else
        {
            Debug.Log("House is unlocked: " + gameObject.name);
        }
    }

    // Method to unlock the house
    public void UnlockHouse()
    {
        isLocked = false;  // Unlock the house if the right conditions are met
        Debug.Log(gameObject.name + " has been unlocked!");
    }

    public void ClosePanel()
    {
        // Hide the panel when the player clicks "No" or closes the window
        QuestionPanel2.SetActive(false);
    }
}
