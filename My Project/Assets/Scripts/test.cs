using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;


public class test : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    private float questionStartTime;
    private int playerScore = 0;
    private int currentQuestionIndex=0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();
    private string apiUrl = "https://localhost:7096/api/users";
    public int predefinedLevel = 1;
   // currentQuestionIndex = questions[currentQuestionIndex].Id;
    // Reference to the HintAgent
    public HintAgent hintAgent;
   // public RLTrainingScript trainingScript;
    void Start()
    {
        StartCoroutine(FetchQuestions(predefinedLevel));

        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() => StartCoroutine(GiveHint(currentQuestionIndex)));
        }
        else
        {
            Debug.LogError("hintButton is not assigned in the Inspector.");
        }

        if (questionText == null) Debug.LogError("questionText is not assigned in the Inspector.");
        if (optionButtons == null || optionButtons.Length == 0) Debug.LogError("optionButtons are not assigned in the Inspector.");
        if (hintText == null) Debug.LogError("hintText is not assigned in the Inspector.");
        if (scoreText == null) Debug.LogError("scoreText is not assigned in the Inspector.");
        if (hintAgent == null) Debug.LogError("hintAgent is not assigned in the Inspector.");

        UpdateScoreText();
    }

    IEnumerator FetchQuestions(int playerLevel)
    {
        string url = $"{apiUrl}/getQuestion?playerLevel={playerLevel}&recommendedLevel={playerLevel}";
        Debug.Log("Connecting to URL: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
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
                    currentQuestionIndex++;
                    DisplayNextUnansweredQuestion();
                    Debug.Log("Question ID: " + questions[currentQuestionIndex].Id); // Log question ID
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

    /*void ResetEnvironmentIfNeeded()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available to start the environment.");
        }
        else
        {
            // Proceed with resetting the environment
            trainingScript.ResetEnvironment();
            //ResetEnvironment();
        }
    }*/


    IEnumerator GiveHint(int questionId)
    {
        string url = $"{apiUrl}/getHint?questionId={questionId}";
        Debug.Log("Connecting to URL: " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                Debug.LogError("Server response: " + webRequest.downloadHandler.text);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                var hints = JsonConvert.DeserializeObject<List<HintModel>>(jsonResponse);

                if (hints != null && hints.Count > 0)
                {
                    HintModel selectedHint = hints[0]; // Select the first hint as an example
                    Debug.Log($"Hint Received: {selectedHint.HintText}");
                    hintText.text = selectedHint.HintText;

                    if (hintAgent != null)
                    {
                        hintAgent.AddReward(-0.1f); // Optional: Give a small penalty for giving a hint
                        hintAgent.EndEpisode();
                    }
                }
                else
                {
                    Debug.LogError("No hints available for this question.");
                }
            }
        }
    }


    void DisplayNextUnansweredQuestion()
    {
        hintText.text = "";

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available to display.");
            return;
        }

        while (currentQuestionIndex < questions.Count && answeredQuestionIds.Contains(questions[currentQuestionIndex].Id))
        {
            currentQuestionIndex++;
        }

        if (currentQuestionIndex < questions.Count)
        {
            DisplayQuestion(questions[currentQuestionIndex]);
        }
        else
        {
            Debug.Log("All questions at this level have been answered.");
        }
    }

    void DisplayQuestion(QuestionModel question)
    {
        if (question == null)
        {
            Debug.LogError("Question is null");
            return;
        }

        if (optionButtons == null || optionButtons.Length < 4)
        {
            Debug.LogError("Option buttons are not correctly set up in the Inspector.");
            return;
        }

        questionStartTime = Time.time;

        questionText.text = question.QuestionText;
        optionButtons[0].GetComponentInChildren<TMP_Text>().text = question.AnswerOption1;
        optionButtons[1].GetComponentInChildren<TMP_Text>().text = question.AnswerOption2;
        optionButtons[2].GetComponentInChildren<TMP_Text>().text = question.AnswerOption3;
        optionButtons[3].GetComponentInChildren<TMP_Text>().text = question.AnswerOption4;

        foreach (Button button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        optionButtons[0].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption1, question.CorrectAnswer, question.Id));
        optionButtons[1].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption2, question.CorrectAnswer, question.Id));
        optionButtons[2].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption3, question.CorrectAnswer, question.Id));
        optionButtons[3].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption4, question.CorrectAnswer, question.Id));
    }

    void OnAnswerSelected(string selectedAnswer, string correctAnswer, int questionId)
    {
        bool answeredCorrectly = selectedAnswer == correctAnswer;

        if (answeredCorrectly)
        {
            playerScore += 10;
            UpdateScoreText();

            if (!answeredQuestionIds.Contains(questionId))
            {
                answeredQuestionIds.Add(questionId);
            }

            if (hintAgent != null)
            {
                hintAgent.AddReward(1.0f); // Reward the agent for a correct answer
                hintAgent.EndEpisode(); // End the current episode for the agent
            }

            currentQuestionIndex++; // Move to the next question only if answered correctly
            DisplayNextUnansweredQuestion();
        }
        else
        {
            if (hintAgent != null)
            {
                hintAgent.AddReward(-1.0f); // Penalize the agent for an incorrect answer
                hintAgent.EndEpisode(); // End the current episode for the agent
            }

            // Optionally provide feedback or keep the current question
            Debug.Log("Incorrect answer. Try again!");
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = playerScore.ToString();
        }
    }

    void ShuffleQuestions()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available to shuffle.");
            return;
        }

        for (int i = 0; i < questions.Count; i++)
        {
            QuestionModel temp = questions[i];
            int randomIndex = Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }
}

[System.Serializable]
public class QuestionModel
{
    public int Id;
    public string QuestionText;
    public string DifficultyLevel;
    public string Category;
    public string Hint;
    public string CorrectAnswer;
    public string AnswerOption1;
    public string AnswerOption2;
    public string AnswerOption3;
    public string AnswerOption4;
    public int RecommendedLevel;
    public List<HintModel> Hints; 
}

[System.Serializable]
public class HintModel
{
    public int HintId;
    public int QuestionId;
    public string HintText;
    public int HintLevel;
}
[System.Serializable]
public class HintListWrapper
{
    public List<HintModel> hints;
}

