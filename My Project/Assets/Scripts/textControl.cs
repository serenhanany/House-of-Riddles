using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Newtonsoft.Json;



public class textControl : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;
    public TMP_Text scoreText;

    private int playerScore = 0;
    private int playerLevel = 1;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();  // Track answered questions
    private string apiUrl = "https://localhost:7096/api/Users/getQuestion";

    void Start()
    {
        StartCoroutine(GetQuestionsForLevel(playerLevel));
        UpdateScoreText();
    }

    public IEnumerator GetQuestionsForLevel(int level)
    {
        string url = $"{apiUrl}?playerLevel={level}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                UnityEngine.Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                UnityEngine.Debug.Log("Received JSON: " + jsonResponse);

                questions = JsonConvert.DeserializeObject<List<QuestionModel>>(jsonResponse);

                if (questions != null && questions.Count > 0)
                {
                    currentQuestionIndex = 0;  // Start with the first question
                    DisplayNextUnansweredQuestion();
                }
                else
                {
                    UnityEngine.Debug.LogError("No questions found for this level.");
                }
            }
        }
    }

    void DisplayNextUnansweredQuestion()
    {
        // Loop through the questions to find one that hasn't been answered
        while (currentQuestionIndex < questions.Count && answeredQuestionIds.Contains(questions[currentQuestionIndex].Id))
        {
            UnityEngine.Debug.Log("Skipping question with ID: " + questions[currentQuestionIndex].Id);
            currentQuestionIndex++;
        }

        if (currentQuestionIndex < questions.Count)
        {
            UnityEngine.Debug.Log("Displaying question with ID: " + questions[currentQuestionIndex].Id);
            DisplayQuestion(questions[currentQuestionIndex]);
        }
        else
        {
            UnityEngine.Debug.Log("All questions at this level have been answered.");
            CheckLevelUp();  // Handle level up or end of quiz
        }
    }

    void DisplayQuestion(QuestionModel question)
    {
        questionText.text = question.QuestionText;
        optionButtons[0].GetComponentInChildren<TMP_Text>().text = question.AnswerOption1;
        optionButtons[1].GetComponentInChildren<TMP_Text>().text = question.AnswerOption2;
        optionButtons[2].GetComponentInChildren<TMP_Text>().text = question.AnswerOption3;
        optionButtons[3].GetComponentInChildren<TMP_Text>().text = question.AnswerOption4;

        // Remove old listeners
        foreach (Button button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        // Add new listeners
        optionButtons[0].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption1, question.CorrectAnswer, question.Id));
        optionButtons[1].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption2, question.CorrectAnswer, question.Id));
        optionButtons[2].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption3, question.CorrectAnswer, question.Id));
        optionButtons[3].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption4, question.CorrectAnswer, question.Id));
    }

    void OnAnswerSelected(string selectedAnswer, string correctAnswer, int questionId)
    {
        if (selectedAnswer == correctAnswer)
        {
            UnityEngine.Debug.Log("Correct Answer!");
            playerScore += 10;  // Increase score
            UpdateScoreText();

            // Track the answered question locally
            if (!answeredQuestionIds.Contains(questionId))
            {
                answeredQuestionIds.Add(questionId);
                UnityEngine.Debug.Log("Added question ID to answered list: " + questionId);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Question ID already in answered list: " + questionId);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Incorrect Answer!");
        }

        currentQuestionIndex++;
        DisplayNextUnansweredQuestion();  // Display the next unanswered question
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + playerScore.ToString();
    }

    void CheckLevelUp()
    {
        if (playerScore >= 50)  // Example: level up after 50 points
        {
            playerLevel++;
            UnityEngine.Debug.Log("Level Up! New Level: " + playerLevel);
            playerScore = 0;  // Reset score or apply other logic

            // Update the player's level in the database
            StartCoroutine(UpdatePlayerLevelInDatabase(playerLevel));

            StartCoroutine(GetQuestionsForLevel(playerLevel));
        }
    }

    IEnumerator UpdatePlayerLevelInDatabase(int newLevel)
    {
        if (PlayerData.Instance == null)
        {
            UnityEngine.Debug.LogError("PlayerData.Instance is null. Cannot update level.");
            yield break;
        }

        int playerId = PlayerData.Instance.playerId;
        if (playerId == 0)  // Assuming playerId of 0 is invalid
        {
            UnityEngine.Debug.LogError("Player ID is not set. Cannot update level.");
            yield break;
        }

        string updateUrl = $"https://localhost:7096/api/Users/updateLevel?newLevel={newLevel}&userId={playerId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Put(updateUrl, ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                UnityEngine.Debug.LogError("Error updating level: " + webRequest.error);
            }
            else
            {
                UnityEngine.Debug.Log("Player level updated successfully.");
            }
        }
    }
}


[System.Serializable]
public class QuestionModel
{
    public int Id { get; set; }  // Primary Key, auto-incremented
    public string QuestionText { get; set; }  // Question text as string
    public string DifficultyLevel { get; set; }  // Difficulty level as string
    public string Category { get; set; }  // Category as string
    public string Hint { get; set; }  // Hint as string
    public string CorrectAnswer { get; set; }  // Correct answer as string
    public string AnswerOption1 { get; set; }  // Answer option 1 as string
    public string AnswerOption2 { get; set; }  // Answer option 2 as string
    public string AnswerOption3 { get; set; }  // Answer option 3 as string
    public string AnswerOption4 { get; set; }  // Answer option 4 as string
}
