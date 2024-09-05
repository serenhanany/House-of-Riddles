using UnityEngine;

public class HouseInteraction2 : MonoBehaviour
{
    public GameObject QuestionPanel2;  // Reference to the panel in the scene
    private bool isLocked = true;  // You can track whether the house is locked

    // This function runs when the player clicks on the house
    private void OnMouseDown()
    {
        if (isLocked)
        {
            // Show the panel when the house is clicked
            QuestionPanel2.SetActive(true);

            // Optionally, you can update the panel content based on the house clicked
            Debug.Log("House clicked: " + gameObject.name);
        }
    }

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

