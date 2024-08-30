
using System.Collections.Generic;
using UnityEngine;

public class RLTrainingScript : MonoBehaviour
{
    public enum RLAction
    {
        NoHint,
        GiveHint
    }

    private QuestionModel currentQuestion;
    private int playerScore;
    private int attempts;
    private bool hintGiven;
    private float timeSpent;

    private List<QuestionModel> questions;

    void Start()
    {
        // Initialize the environment with a set of questions, player data, etc.
        ResetEnvironment();
    }

    public void ResetEnvironment()
    {
        // Reset all relevant variables to start a new episode
        playerScore = 0;
        attempts = 0;
        hintGiven = false;
        timeSpent = 0;
        LoadQuestions();
        ChooseNewQuestion();
    }

    private void LoadQuestions()
    {
        // Load questions from a data source or use predefined questions
        questions = new List<QuestionModel>
        {
            new QuestionModel { Id = 1, QuestionText = "What is 2 + 2?", CorrectAnswer = "4", Hint = "Think about basic math.", AnswerOption1 = "3", AnswerOption2 = "4", AnswerOption3 = "5", AnswerOption4 = "6" },
            new QuestionModel { Id = 2, QuestionText = "What is the capital of France?", CorrectAnswer = "Paris", Hint = "It's known as the city of love.", AnswerOption1 = "Rome", AnswerOption2 = "Madrid", AnswerOption3 = "Paris", AnswerOption4 = "Berlin" },
            // Add more questions here
        };
    }

    public Dictionary<string, float> GetState()
    {
        // Collect relevant state data (e.g., number of attempts, hint given, etc.)
        return new Dictionary<string, float>
        {
            { "Attempts", attempts },
            { "HintGiven", hintGiven ? 1f : 0f },
            { "TimeSpent", timeSpent }
        };
    }

    public void TakeAction(RLAction action)
    {
        // Apply the selected action in the environment
        if (action == RLAction.GiveHint)
        {
            GiveHint();
        }
    }

    private void GiveHint()
    {
        // Logic for giving a hint to the player
        hintGiven = true;
        Debug.Log($"Hint Given: {currentQuestion.Hint}");
    }

    public bool CheckIfAnswerCorrect()
    {
        // Check if the player's answer is correct (this would depend on your game logic)
        return currentQuestion.CorrectAnswer == "4"; // Example check, replace with actual answer checking logic
    }

    public float GetReward(bool answeredCorrectly)
    {
        // Define a reward function based on whether the answer was correct and other factors
        if (answeredCorrectly)
        {
            return 1.0f;
        }
        else if (hintGiven)
        {
            return -0.5f; // Penalty for needing a hint
        }
        else
        {
            return -1.0f;
        }
    }

    public bool IsEpisodeDone()
    {
        // Define the condition for ending an episode
        return attempts >= 3; // Example condition: end episode after 3 attempts
    }

    private void ChooseNewQuestion()
    {
        // Choose a new question randomly from the list
        int randomIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomIndex];
        Debug.Log($"New Question: {currentQuestion.QuestionText}");
    }
}
