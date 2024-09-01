using UnityEngine;

public class HouseLock : MonoBehaviour
{
    public bool isLocked = true;  // Set this to true to lock the house by default
    public string unlockMessage = "This house is locked!";  // Message to display when the house is locked

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLocked)
            {
                Debug.Log(unlockMessage);
                // Optionally, display a UI message here
            }
            else
            {
                Debug.Log("The house is unlocked! You may enter.");
                // Add code here to allow the player to enter the house, such as opening a door
            }
        }
    }

    // Function to unlock the house (call this function when the player meets the unlocking condition)
    public void UnlockHouse()
    {
        isLocked = false;
        Debug.Log("The house has been unlocked!");
    }
}

