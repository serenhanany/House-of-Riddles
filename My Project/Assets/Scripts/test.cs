using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;






public class test : MonoBehaviour
{
    //public TMP_Text playerLevelText;
    public bool questionsLoaded = false;
    public TMP_Text questionText;
    public TMP_Text InCorrect;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    private float questionStartTime;
    private int playerScore = 0;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    private HashSet<int> answeredQuestionIds = new HashSet<int>();
    private string apiUrl = "https://localhost:7096/api/users";
    public GameObject QuestionPanel2;
    //public int predefinedLevel = 1;
    // currentQuestionIndex = questions[currentQuestionIndex].Id;
    // Reference to the HintAgent
    // public HintAgent hintAgent;
    // public RLTrainingScript trainingScript;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //playerLevelText.text = $"Level: {PlayerData.Instance.playerLevel}";
        StartCoroutine(FetchQuestionsFromDB(PlayerData.Instance.playerLevel));

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
        //if (hintAgent == null) Debug.LogError("hintAgent is not assigned in the Inspector.");

        UpdateScoreText();
    }


    IEnumerator FetchQuestionsFromDB(int playerLevel)
    {
        string url = $"{apiUrl}/getQuestion";  // No player level specified, fetch all questions
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

        foreach (var question in questions)
        {
            Debug.Log("Fetching hints for question ID: " + question.Id);

            string hintsUrl = $"{apiUrl}/getHint?questionId={question.Id}";
            UnityWebRequest hintsRequest = UnityWebRequest.Get(hintsUrl);
            yield return hintsRequest.SendWebRequest();

            if (hintsRequest.result == UnityWebRequest.Result.ConnectionError || hintsRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching hints for question ID: {question.Id} - {hintsRequest.error}. Skipping this question's hints.");
                continue; // Continue to next question even if fetching hints fails
            }

            List<HintModel> hints = JsonConvert.DeserializeObject<List<HintModel>>(hintsRequest.downloadHandler.text);
            question.Hints = hints;
            Debug.Log("Total hint fetched: " + hints.Count);
            if (question.Hints == null || question.Hints.Count < 3)
            {
                Debug.LogError($"Question {question.Id} does not have enough hints. Skipping this question's hints.");
                continue;
            }
        }
        DisplayNextUnansweredQuestion();

         }
    IEnumerator GiveHint(int hintLevel)
    {
        // Check if questions have been fetched and the current index is within bounds
        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("Questions list is null or empty.");
            yield break;
        }

        if (currentQuestionIndex >= questions.Count)
        {
            Debug.LogError("Questions list is out of bounds. Check if the currentQuestionIndex is correct.");
            yield break;
        }

        QuestionModel currentQuestion = questions[currentQuestionIndex];

        // Check if there are hints available for the current question
        if (currentQuestion.Hints == null || currentQuestion.Hints.Count == 0)
        {
            Debug.LogError("No hints available for this question.");
            yield break;
        }

        // Randomly select a hint from the available hints
        int randomIndex = Random.Range(0, currentQuestion.Hints.Count);
        HintModel selectedHint = currentQuestion.Hints[randomIndex];

        if (selectedHint != null)
        {
            hintText.text = selectedHint.HintText; // Display the hint in the UI
                                                   // hintAgent.AddReward(-0.01f); // Optional: Small penalty for using a hint
                                                   // hintAgent.EndEpisode(); // End the episode after hint is given
        }
        else
        {
            Debug.LogError("Selected hint is null.");
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
                /*
                if (hintAgent != null)
                {
                    hintAgent.AddReward(1.0f); // Reward the agent for a correct answer
                    hintAgent.EndEpisode(); // End the current episode for the agent
                }
                */
                currentQuestionIndex++; // Move to the next question only if answered correctly
            InCorrect.text = "";
            QuestionPanel2.SetActive(false);
            DisplayNextUnansweredQuestion();

        }
            else
            {/*
                if (hintAgent != null)
                {
                    hintAgent.AddReward(-1.0f); // Penalize the agent for an incorrect answer
                    hintAgent.EndEpisode(); // End the current episode for the agent
                }*/

            // Optionally provide feedback or keep the current question
              InCorrect.text="In Correct Answer, Try again";
            Debug.Log("Incorrect answer. Try again!");
            }
        }

        void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: "+playerScore.ToString();
                PlayerData.Instance.Score = playerScore;
            }
        }

        /*void ShuffleQuestions()
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
        }*/
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

