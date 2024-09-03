using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    public Question currentQuestion; // Field to hold the current question

    // Define the Question class within the QuestionManager
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;

        public Question(string questionText, string[] answers, int correctAnswerIndex)
        {
            this.questionText = questionText;
            this.answers = answers;
            this.correctAnswerIndex = correctAnswerIndex;
        }
    }

    private void OnMouseDown()
    {
        DisplayQuestion();
    }

    public void DisplayQuestion()
    {
        if (questionText == null || answerButtons == null || answerButtons.Length == 0)
        {
            Debug.LogError("QuestionManager fields are not assigned properly.");
            return;
        }

        // Display the question text
        questionText.text = currentQuestion.questionText;

        // Loop through the answer buttons and assign the answer text
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
                int index = i; // Capture index to pass to the listener
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false); // Hide unused buttons
            }
        }
    }

    public void CheckAnswer(int index)
    {
        // Check if the selected answer is correct
        if (index == currentQuestion.correctAnswerIndex)
        {
            Debug.Log("Correct Answer!");
            // Additional logic for correct answer can go here
        }
        else
        {
            Debug.Log("Wrong Answer!");
            // Additional logic for wrong answer can go here
        }
    }
}

