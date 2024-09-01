using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

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

    private string apiUrl = "https://localhost:7096/api/users"; // Base API URL
    public int predefinedLevel = 1;  // This could be dynamic based on player data

    void Start()
    {
        // Initialize the environment with a set of questions, player data, etc.
        StartCoroutine(LoadQuestionsFromDB(predefinedLevel));
    }

    public void ResetEnvironment()
    {
        // Reset all relevant variables to start a new episode
        playerScore = 0;
        attempts = 0;
        hintGiven = false;
        timeSpent = 0;
        if (questions != null && questions.Count > 0)
        {
            ChooseNewQuestion();
        }
        else
        {
            Debug.LogError("No questions available to start the environment.");
        }
    }

    private IEnumerator LoadQuestionsFromDB(int playerLevel)
    {
        string url = $"{apiUrl}/getQuestions?playerLevel={playerLevel}";
        Debug.Log("Connecting to URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Server response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                questions = JsonConvert.DeserializeObject<List<QuestionModel>>(request.downloadHandler.text);

                if (questions != null && questions.Count > 0)
                {
                    Debug.Log("Questions loaded successfully!");
                    ResetEnvironment();
                }
                else
                {
                    Debug.LogError("No questions found or failed to parse JSON.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch questions: " + request.downloadHandler.text);
            }
        }
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
