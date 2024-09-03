using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
    public QuestionManager questionManager; // Reference to the QuestionManager

    private void OnMouseDown()
    {
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager is not assigned in HouseInteraction.");
            return;
        }

        questionManager.DisplayQuestion();
    }
}



