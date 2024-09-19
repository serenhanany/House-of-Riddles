using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;



public class textControl : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    private float questionStartTime;
    private int playerScore = 0;
    private int playerLevel;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();
    private string apiUrl = "https://localhost:7096/api/Users";

    // Reference to the HintAgent
    public HintAgent hintAgent;

    void Start()
    {
        // Check PlayerData.Instance
        if (PlayerData.Instance == null)
        {
            Debug.LogError("PlayerData.Instance is null. Ensure it is initialized before accessing it.");
            return;
        }

        playerLevel = PlayerData.Instance.playerLevel;
        StartCoroutine(GetQuestionsForLevel(playerLevel));

        // Check UI Elements
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

    IEnumerator GiveHint(int questionId)
    {
        string url = $"{apiUrl}/getHint?questionId={questionId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error fetching hint: " + webRequest.error);
                yield break;
            }
            else
            {
                string hint = webRequest.downloadHandler.text;
                hintText.text = hint;
                PlayerData.Instance.HintGiven = true;

                // Inform the HintAgent that a hint was given
                if (hintAgent != null)
                {
                    hintAgent.AddReward(-0.1f); // Optional: Give a small penalty for giving a hint
                    hintAgent.EndEpisode();
                }

                StartCoroutine(UpdatePlayerData(
                    PlayerData.Instance.playerId,
                    questionId,
                    false,
                    PlayerData.Instance.CurrentQuestionAttempts,
                    PlayerData.Instance.HintGiven,
                    PlayerData.Instance.TimeSpentOnCurrentQuestion
                ));
            }
        }
    }

    public IEnumerator GetQuestionsForLevel(int level)
    {
        string url = $"{apiUrl}/getQuestion?playerLevel={level}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Received JSON: " + jsonResponse);

                questions = JsonConvert.DeserializeObject<List<QuestionModel>>(jsonResponse);

                if (questions != null && questions.Count > 0)
                {
                    ShuffleQuestions();
                    currentQuestionIndex = 0;
                    DisplayNextUnansweredQuestion();
                }
                else
                {
                    Debug.LogError("No questions found for this level.");
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
            CheckLevelUp();
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
        PlayerData.Instance.TimeSpentOnCurrentQuestion = 0;
        PlayerData.Instance.CurrentQuestionAttempts = 0;
        PlayerData.Instance.HintGiven = false;

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
        PlayerData.Instance.TimeSpentOnCurrentQuestion = Time.time - questionStartTime;
        PlayerData.Instance.CurrentQuestionAttempts++;

        bool answeredCorrectly = selectedAnswer == correctAnswer;

        if (answeredCorrectly)
        {
            playerScore += 10;
            UpdateScoreText();
            CheckLevelUp();
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

        // Update player data regardless of correctness
        StartCoroutine(UpdatePlayerData(
            PlayerData.Instance.playerId,
            questionId,
            answeredCorrectly,
            PlayerData.Instance.CurrentQuestionAttempts,
            PlayerData.Instance.HintGiven,
            PlayerData.Instance.TimeSpentOnCurrentQuestion
        ));
    }


    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = playerScore.ToString();
        }
    }

    void CheckLevelUp()
    {
        if (playerScore >= 50)
        {
            playerLevel++;
            playerScore = 0;
            StartCoroutine(UpdatePlayerLevelInDatabase(playerLevel));
            StartCoroutine(GetQuestionsForLevel(playerLevel));
        }
    }

    IEnumerator UpdatePlayerLevelInDatabase(int newLevel)
    {
        if (PlayerData.Instance == null || PlayerData.Instance.playerId == 0)
        {
            yield break;
        }

        string updateUrl = $"{apiUrl}/updateLevel?newLevel={newLevel}&userId={PlayerData.Instance.playerId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Put(updateUrl, ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error updating level: " + webRequest.error);
            }
            else
            {
                Debug.Log("Player level updated successfully.");
            }
        }
    }

    IEnumerator UpdatePlayerData(int userId, int questionId, bool answeredCorrectly, int attempts, bool hintGiven, float timeTaken)
    {
        Debug.Log($"Sending data: userId={userId}, questionId={questionId}, answeredCorrectly={answeredCorrectly}, attempts={attempts}, hintGiven={hintGiven}, timeTaken={timeTaken}");

        string url = $"{apiUrl}/updatePerformance?userId={userId}&questionId={questionId}&answeredCorrectly={answeredCorrectly}&attempts={attempts}&hintGiven={hintGiven}&timeTaken={timeTaken}";

        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error updating player performance: " + webRequest.error);
            }
            else
            {
                Debug.Log("Player performance updated successfully.");
            }
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
