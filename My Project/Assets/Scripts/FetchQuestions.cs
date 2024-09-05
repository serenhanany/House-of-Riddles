using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;




public class FetchQuestions : MonoBehaviour
{
    private string selectedAnswer;
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    private int playerScore = 0;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();
    private string apiUrl = "https://localhost:7096/api/users";
    public int predefinedLevel = 1;
    private float questionStartTime;

    // Reference to the RL agent for hinting
    public HintAgent hintAgent;

    void Start()
    {
        StartCoroutine(FetchQuestionsFromDB(predefinedLevel));
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() => StartCoroutine(GiveHint(currentQuestionIndex)));
        }
        else
        {
            Debug.LogError("Hint button is not assigned.");
        }
        UpdateScoreText();
    }


    // Fetch questions based on player level
    IEnumerator FetchQuestionsFromDB(int playerLevel)
    {
        string url = $"{apiUrl}/getQuestion?playerLevel={playerLevel}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching questions: " + request.error);
        }
        else
        {
            questions = JsonConvert.DeserializeObject<List<QuestionModel>>(request.downloadHandler.text);
            // Ensure each question has exactly 3 hints
            foreach (var question in questions)
            {
                string hintsUrl = $"{apiUrl}/getHintsForQuestion?questionId={question.Id}";
                UnityWebRequest hintsRequest = UnityWebRequest.Get(hintsUrl);
                yield return hintsRequest.SendWebRequest();

                if (hintsRequest.result == UnityWebRequest.Result.ConnectionError || hintsRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error fetching hints: " + hintsRequest.error);
                }
                else
                {
                    List<HintModel> hints = JsonConvert.DeserializeObject<List<HintModel>>(hintsRequest.downloadHandler.text);
                    question.Hints = hints;

                    if (question.Hints == null || question.Hints.Count < 3)
                    {
                        Debug.LogError($"Question {question.Id} does not have enough hints.");
                    }
                }
            }

            DisplayNextUnansweredQuestion();
        }
    }

    // Fetch hint based on question ID
    public IEnumerator GiveHint(int questionId)
    {
        // The agent decides which hint level to give
        int hintLevel = hintAgent.DecideHintLevel(); // Implement this in HintAgent

        if (questions != null && currentQuestionIndex < questions.Count)
        {
            QuestionModel currentQuestion = questions[currentQuestionIndex];

            // Fetch the appropriate hint based on hintLevel
            HintModel selectedHint = currentQuestion.Hints.Find(h => h.HintLevel == hintLevel);
            if (selectedHint != null)
            {
                hintText.text = selectedHint.HintText; // Display the hint in the UI
                hintAgent.AddReward(-0.1f); // Optional: Penalty for using a hint
                hintAgent.EndEpisode();
            }
            else
            {
                Debug.LogError("No hints available for this question.");
            }
        }

        yield break; // Ensure all code paths return a value
    }


    // Display the next unanswered question
    void DisplayNextUnansweredQuestion()
    {
        hintText.text = "";

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available.");
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
            Debug.Log("All questions answered.");
        }
    }

    // Display a specific question
    void DisplayQuestion(QuestionModel question)
    {
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

    // Handle the selected answer
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

            hintAgent.AddReward(1.0f); // Reward the agent
            hintAgent.EndEpisode(); // End the episode for the agent
            currentQuestionIndex++;
            DisplayNextUnansweredQuestion();
        }
        else
        {
            hintAgent.AddReward(-1.0f); // Penalize for wrong answer
            hintAgent.EndEpisode(); // End the episode for the agent
            Debug.Log("Incorrect answer.");
        }
    }

    // Update the player's score on the UI
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public QuestionModel GetCurrentQuestion()
    {
        if (questions != null && currentQuestionIndex < questions.Count)
        {
            return questions[currentQuestionIndex];
        }
        return null;
    }

    // This method will check if the player's selected answer is correct
    public bool CheckIfAnswerCorrect(string selectedAnswer)
    {
        if (questions != null && currentQuestionIndex < questions.Count)
        {
            return selectedAnswer == questions[currentQuestionIndex].CorrectAnswer;
        }
        return false;
    }
    public string GetSelectedAnswer()
    {
        return selectedAnswer; // Return the answer that was selected by the player
    }

    // Call this method when the player selects an answer
    public void SetSelectedAnswer(string answer)
    {
        selectedAnswer = answer; // Store the player's selected answer
    }
}

