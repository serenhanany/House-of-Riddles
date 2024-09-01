using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class test : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    private float questionStartTime;
    private int playerScore = 0;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();

    // Reference to the HintAgent
    public HintAgent hintAgent;

    void Start()
    {
        // Load a predefined set of questions directly for training purposes
        LoadQuestions();

        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() => GiveHint(currentQuestionIndex));
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

    void GiveHint(int questionId)
    {
        // Simulate hint being given for training
        string hint = "This is a hint.";
        hintText.text = hint;

        // Inform the HintAgent that a hint was given
        if (hintAgent != null)
        {
            hintAgent.AddReward(-0.1f); // Optional: Give a small penalty for giving a hint
            hintAgent.EndEpisode();
        }
    }

    void LoadQuestions()
    {
        // Simulate loading a set of predefined questions for training
        questions = new List<QuestionModel>
        {
            new QuestionModel { Id = 1, QuestionText = "What is 2 + 2?", AnswerOption1 = "3", AnswerOption2 = "4", AnswerOption3 = "5", AnswerOption4 = "6", CorrectAnswer = "4" },
            new QuestionModel { Id = 2, QuestionText = "What is the capital of France?", AnswerOption1 = "Berlin", AnswerOption2 = "Paris", AnswerOption3 = "Rome", AnswerOption4 = "Madrid", CorrectAnswer = "Paris" }
            // Add more questions as needed
        };

        ShuffleQuestions();
        currentQuestionIndex = 0;
        DisplayNextUnansweredQuestion();
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
            Debug.Log("All questions have been answered.");
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
/*
[System.Serializable]
public class QuestionModel
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CorrectAnswer { get; set; }
    public string AnswerOption1 { get; set; }
    public string AnswerOption2 { get; set; }
    public string AnswerOption3 { get; set; }
    public string AnswerOption4 { get; set; }
}*/
