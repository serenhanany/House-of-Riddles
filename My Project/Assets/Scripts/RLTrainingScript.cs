using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

using System.Linq;


public class RLTrainingScript : MonoBehaviour
{
    public enum RLAction
    {
        NoHint,
        GiveHint
    }
    public QuestionModel CurrentQuestion { get; private set; }
    //private QuestionModel currentQuestion;
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

    

    private IEnumerator LoadQuestionsFromDB(int playerLevel)
    {
        
        //string url = $"{apiUrl}/getQuestions?playerLevel={playerLevel}";
        string url = $"{apiUrl}/getQuestion?playerLevel={playerLevel}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
           
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Log the received data for debugging
           // Debug.Log("Received questions data: " + request.downloadHandler.text);
            
            questions = JsonConvert.DeserializeObject<List<QuestionModel>>(request.downloadHandler.text);

            foreach (var question in questions)
            {
                // Fetch associated hints for the question
                string hintsUrl = $"{apiUrl}/getHint?questionId={question.Id}";
                UnityWebRequest hintsRequest = UnityWebRequest.Get(hintsUrl);
                yield return hintsRequest.SendWebRequest();

                if (hintsRequest.result == UnityWebRequest.Result.ConnectionError || hintsRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error fetching hints: " + hintsRequest.error);
                }
                else
                {
                    // Log the received hints data for debugging
                   // Debug.Log($"Received hints data for Question ID {question.Id}: {hintsRequest.downloadHandler.text}");

                    question.Hints = JsonConvert.DeserializeObject<List<HintModel>>(hintsRequest.downloadHandler.text);
                }
            }

            // Log a confirmation that all data has been processed
            Debug.Log("All questions and hints have been successfully loaded.");
            Debug.Log("Number of questions loaded: " + (questions != null ? questions.Count : 0));
            if (questions != null && questions.Count > 0)
            {
                ResetEnvironment();
            }
            else
            {
                Debug.LogError("Questions list is empty or null, cannot start environment.");
            }

        }
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
            HintModel selectedHint = ChooseHint(); // ChooseHint() should be your method to select the appropriate hint
            GiveHint(selectedHint);
        }
    }

    
    public void GiveHint(HintModel selectedHint)
    {
        // Logic for giving a hint to the player
        hintGiven = true;
        Debug.Log($"Hint Given: {selectedHint.HintText}");
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
   
    private HintModel ChooseHint()
    {
        // Assuming you have a list of hints for the current question
        List<HintModel> availableHints = GetHintsForCurrentQuestion();

        // Implement your logic to choose the appropriate hint.
        // For this example, we'll select the hint with the lowest HintLevel (easiest hint).
        if (availableHints != null && availableHints.Count > 0)
        {
            HintModel selectedHint = availableHints
                .OrderBy(h => h.HintLevel) // Choose the easiest hint available
                .FirstOrDefault(); // Get the first hint from the sorted list

            return selectedHint;
        }

        Debug.LogWarning("No hints available for the current question.");
        return null;
    }

    // Mock method to simulate fetching hints for the current question
    private List<HintModel> GetHintsForCurrentQuestion()
    {
        // Replace with actual logic to fetch hints from your database or data source
        return new List<HintModel>
    {
        new HintModel { HintText = "Basic hint", HintLevel = 1 },
        new HintModel { HintText = "Intermediate hint", HintLevel = 2 },
        new HintModel { HintText = "Advanced hint", HintLevel = 3 }
    };
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool CheckIfAnswerCorrect()
    {
        // Check if the player's answer is correct (this would depend on your game logic)
        return CurrentQuestion.CorrectAnswer == "4"; // Example check, replace with actual answer checking logic
    }

    public float GetReward(bool answeredCorrectly, int hintLevel)
    {
        // Define a reward function based on whether the answer was correct and other factors
        if (answeredCorrectly)
        {
            return 1.0f + 0.1f * hintLevel;  // Small bonus for correct answers with lower-level hints
        }
        else if (hintGiven)
        {
            return -0.5f - 0.1f * hintLevel;  // Penalty increases with more helpful hints
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
        CurrentQuestion = questions[randomIndex];
        Debug.Log($"New Question: {CurrentQuestion.QuestionText}");
    }

}
