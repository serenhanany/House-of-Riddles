using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;
using TMPro;





public class RLTrainingScript : MonoBehaviour
{
    public enum RLAction
    {
        NoHint,
        GiveHint
    }

    public QuestionModel CurrentQuestion { get; private set; }
    private int playerScore;
    private int attempts;
    private bool hintGiven;
    private float timeSpent;
    public TMP_Text hintText;
    private List<QuestionModel> questions;
    public List<QuestionModel> Questions  // Public property to access the list
    {
        get { return questions; }
    }
    private string apiUrl = "https://localhost:7096/api/users"; // Base API URL
    public int predefinedLevel = 1;

    void Start()
    {
        // Initialize the environment with a set of questions, player data, etc.
        StartCoroutine(LoadQuestionsFromDB(predefinedLevel));
    }

    private IEnumerator LoadQuestionsFromDB(int playerLevel)
    {
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
                    question.Hints = JsonConvert.DeserializeObject<List<HintModel>>(hintsRequest.downloadHandler.text);
                }
            }

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
        Debug.Log("ResetEnvironment called.");

        // Ensure player-related variables are reset
        playerScore = 0;
        attempts = 0;
        hintGiven = false;
        timeSpent = 0;

        if (questions == null)
        {
            Debug.LogError("Questions list is null.");
            return;
        }

        if (questions.Count == 0)
        {
            Debug.LogError("Questions list is empty.");
            return;
        }

        // Randomly select a new question
        ChooseNewQuestion();
        Debug.Log("New question selected.");
    }


    public Dictionary<string, float> GetState()
    {
        return new Dictionary<string, float>
        {
            { "Attempts", attempts },
            { "HintGiven", hintGiven ? 1f : 0f },
            { "TimeSpent", timeSpent }
        };
    }

    public void TakeAction(RLAction action)
    {
        if (action == RLAction.GiveHint)
        {
            HintModel selectedHint = ChooseHint(); // ChooseHint() selects the appropriate hint
            GiveHint(selectedHint);
        }
    }

    public void GiveHint(HintModel selectedHint)
    {
        if (selectedHint != null)
        {
            hintGiven = true;
            Debug.Log($"Hint Given: {selectedHint.HintText}");

            // Assuming you have a UI Text component to display the hint
            hintText.text = selectedHint.HintText; // Update this to your actual hint display component
        }
        else
        {
            Debug.LogWarning("No hint was selected or available to give.");
        }
    }


    private HintModel ChooseHint()
    {
        List<HintModel> availableHints = GetHintsForCurrentQuestion();

        if (availableHints != null && availableHints.Count > 0)
        {
            HintModel selectedHint = availableHints
                .OrderBy(h => h.HintLevel) // Choose the easiest hint available
                .FirstOrDefault();

            return selectedHint;
        }

        Debug.LogWarning("No hints available for the current question.");
        return null;
    }

    private List<HintModel> GetHintsForCurrentQuestion()
    {
        if (CurrentQuestion == null)
        {
            Debug.LogError("CurrentQuestion is null.");
            return null;
        }

        if (CurrentQuestion.Hints == null || CurrentQuestion.Hints.Count == 0)
        {
            Debug.LogError("No hints available for the current question.");
            return null;
        }

        return CurrentQuestion.Hints;
    }


    public bool CheckIfAnswerCorrect()
    {
        // Replace this example check with your actual answer validation logic
        return CurrentQuestion.CorrectAnswer == "4";
    }

    public float GetReward(bool answeredCorrectly, int hintLevel)
    {
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
        // Example condition: end episode after 3 attempts
        return attempts >= 3;
    }

    private void ChooseNewQuestion()
    {
        int randomIndex = Random.Range(0, questions.Count);
        CurrentQuestion = questions[randomIndex];
        Debug.Log($"New Question: {CurrentQuestion.QuestionText}");
    }
}
