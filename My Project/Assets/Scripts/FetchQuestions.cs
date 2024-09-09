using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;






public class FetchQuestions : MonoBehaviour
{
    public bool questionsLoaded = false;
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
        // Fetch questions from the server or database based on the player's predefined level
        StartCoroutine(FetchQuestionsFromDB(predefinedLevel));

        // No need to use hintButton since the RL agent decides when to give a hint
        if (hintButton != null)
        {
            hintAgent.enabled = false;
            Debug.Log("Hint button is currently disabled as the RL agent controls hint-giving.");
        }

        UpdateScoreText();
    }





    // Fetch questions based on player level
    IEnumerator FetchQuestionsFromDB(int playerLevel)
    {
        string url = $"{apiUrl}/getQuestion";  // No player level specified, fetch all questions

        //string url = $"{apiUrl}/getQuestion?playerLevel={playerLevel}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching questions: " + request.error);
            questionsLoaded = false;
            yield break; // Exit the coroutine if there is an error
        }

        // Deserialize the questions from the server response
        questions = JsonConvert.DeserializeObject<List<QuestionModel>>(request.downloadHandler.text);

        // Ensure we got valid questions
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions were returned or the question data is invalid.");
            questionsLoaded = false;
            yield break; // Exit the coroutine if no questions are available
        }
        Debug.Log("Total questions fetched: " + questions.Count);

        questionsLoaded = true;

        if (hintAgent != null)
        {
            hintAgent.enabled = true;  // Re-enable the agent's decision-making
            hintAgent.OnEpisodeBegin();  // Manually trigger OnEpisodeBegin() again to start the episode
        }


        foreach (var question in questions)
        {

            Debug.Log("Fetching hints for question ID: " + question.Id);

            string hintsUrl = $"{apiUrl}/getHint?questionId={question.Id}";
            UnityWebRequest hintsRequest = UnityWebRequest.Get(hintsUrl);
            yield return hintsRequest.SendWebRequest();

            if (hintsRequest.result == UnityWebRequest.Result.ConnectionError || hintsRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching hints for question ID: {question.Id} - {hintsRequest.error}");
                yield break; // Handle error in fetching hints
            }

            List<HintModel> hints = JsonConvert.DeserializeObject<List<HintModel>>(hintsRequest.downloadHandler.text);
            question.Hints = hints;

            if (question.Hints == null || question.Hints.Count < 3)
            {
                Debug.LogError($"Question {question.Id} does not have enough hints.");
                yield break;
            }
        }
        Debug.Log("All questions and hints have been processed.");
        // After fetching the questions and hints, display the next unanswered question
        DisplayNextUnansweredQuestion();
    }


    public void GiveHint(int hintLevel)
    {
        // Check if questions have been fetched and the current index is within bounds
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("Questions list is null or empty.");
            return;
        }

        if (currentQuestionIndex >= questions.Count)
        {
            Debug.LogError("Questions list is out of bounds. Check if the currentQuestionIndex is correct.");
            return;
        }

        QuestionModel currentQuestion = questions[currentQuestionIndex];

        // Fetch the appropriate hint based on the hintLevel provided by the RL agent
        HintModel selectedHint = currentQuestion.Hints.Find(h => h.HintLevel == hintLevel);
        if (selectedHint != null)
        {
            hintText.text = selectedHint.HintText; // Display the hint in the UI
            hintAgent.AddReward(-0.1f); // Optional: Small penalty for using a hint
            hintAgent.EndEpisode(); // End the episode after hint is given
        }
        else
        {
            Debug.LogError("No hints available for this question.");
        }
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
            Debug.Log("All questions answered. Total questions answered: " + answeredQuestionIds.Count);
            EndGame();
        }
    }

    // Display a specific question
    void DisplayQuestion(QuestionModel question)
    {
        if (question == null)
        {
            Debug.LogError("Attempted to display a null question.");
            return;
        }

        Debug.Log("Displaying question index: " + currentQuestionIndex + ", Question ID: " + question.Id);
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
            if (currentQuestionIndex < questions.Count - 1)
            {
                currentQuestionIndex++;
                Debug.Log("Moving to next question. Current question index: " + currentQuestionIndex);
                DisplayNextUnansweredQuestion();
            }
            else
            {
                Debug.Log("All questions answered.");
                EndGame(); // If no more questions are left, handle game end
            }
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
        scoreText.text =playerScore.ToString();
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
    void EndGame()
    {
        Debug.Log("All questions have been answered. The game is over.");

        // Optionally, you could display a message to the player
        questionText.text = "Quiz Complete! Your final score is: " + playerScore;

        // disable buttons to prevent further input
        foreach (Button button in optionButtons)
        {
            button.interactable = false;
        }

    }

}

