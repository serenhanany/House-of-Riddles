using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;





public class FetchQuestions : MonoBehaviour
{
    private List<float> questionTimes = new List<float>();  // Track the time spent on each question
    private int correctFirstAttemptCount = 0;  // Tracks the number of first-time correct answers
    private int totalQuestionsAnswered = 0;    // Tracks the total number of questions answered
   // public GameObject QuestionPanel2;
    public int maxAttemptsPerQuestion = 3;  
    public int AttemptsRemaining { get;  set; }  
    public bool questionsLoaded = false;
    private string selectedAnswer;
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Button hintButton;
    public TMP_Text hintText;
    public TMP_Text scoreText;
    public int playerScore = 0;
    private int currentQuestionIndex = 0;
    private List<QuestionModel> questions;
    public List<QuestionModel> Questions { get { return questions; } }
    public QuestionModel currentQuestion { get; private set; }
    public HashSet<int> answeredQuestionIds = new HashSet<int>();
    private string apiUrl = "https://localhost:7096/api/users";
    public int predefinedLevel;
    public float questionStartTime;
    public float maxAllowedTime = 30.0f;
    // Reference to the RL agent for hinting
    public HintAgent hintAgent;

    void Start()
    {
        // Fetch questions from the server or database based on the player's predefined level
       
        //predefinedLevel = PlayerData.Instance.playerLevel;
        StartCoroutine(FetchQuestionsFromDB(1));
        AttemptsRemaining = maxAttemptsPerQuestion;
        // No need to use hintButton since the RL agent decides when to give a hint
        /* if (hintButton != null)
         {
             hintButton.onClick.AddListener(OnHintButtonPressed);
             hintButton.interactable = true; // Make the button clickable
             Debug.Log("Hint button is now enabled for the player to press.");
         }*/
        if (hintButton != null)
        {
            hintAgent.enabled = false;
            Debug.Log("Hint button is currently disabled as the RL agent controls hint-giving.");
        }

        UpdateScoreText();
    }
    void OnHintButtonPressed()
    {
        if (hintAgent != null)
        {
            Debug.Log("Hint button pressed. Agent is making a decision.");
            hintAgent.RequestDecision();  // Request the agent to make a decision
        }
    }
    public void ResetAttempts()
    {
        AttemptsRemaining = maxAttemptsPerQuestion;
    }

    public bool MoveToNextUnansweredQuestion()
    {
        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            ResetAttempts();  // Reset attempts when moving to the next question
            DisplayNextUnansweredQuestion();
            return true;
        }
        else
        {
            Debug.Log("No more unanswered questions.");
            return false;  // No more questions
        }
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
            //Debug.Log("Fetching hints for question ID: " + question.Id);

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

            if (question.Hints == null || question.Hints.Count < 3)
            {
                Debug.LogError($"Question {question.Id} does not have enough hints. Skipping this question's hints.");
                continue;
            }
        }

        questionsLoaded = true; // Set to true after all questions and hints are fetched
        Debug.Log("All questions and hints have been processed.");

        // Re-enable the agent if it's disabled
        if (hintAgent != null && !hintAgent.enabled && questionsLoaded)
        {
            hintAgent.enabled = true;  // Re-enable the agent's decision-making
            hintAgent.OnEpisodeBegin();  // Manually trigger OnEpisodeBegin() again to start the episode
        }

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
            switch (hintLevel)
            {
                case 1: // Easy hint
                    hintAgent.AddReward(-0.5f);
                    break;
                case 2: // Medium hint
                    hintAgent.AddReward(-1.0f);
                    break;
                case 3: // Hard hint
                    hintAgent.AddReward(-1.5f);
                    break;
            }

            hintAgent.EndEpisode(); // End the episode after hint is given
        }
        else
        {
            Debug.LogError("No hints available for this question.");
        }
    }




    // Display the next unanswered question
    public void DisplayNextUnansweredQuestion()
    {
       // hintText.text = "";

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError("No questions available.");
            return;
        }
        //Debug.Log($"Initial currentQuestionIndex: {currentQuestionIndex}, Questions Count: {questions.Count}");
/*
        if (!answeredQuestionIds.Contains(questions[currentQuestionIndex].Id))
        {
            answeredQuestionIds.Add(questions[currentQuestionIndex].Id);
        }*/
        // Check the contents of answeredQuestionIds
        //Debug.Log("Answered Question IDs: " + string.Join(", ", answeredQuestionIds));

        // Check if the current question is in answeredQuestionIds
        if (currentQuestionIndex < questions.Count)
        {
            Debug.Log($"Current Question ID: {questions[currentQuestionIndex].Id}");
        }

        while (currentQuestionIndex < questions.Count && answeredQuestionIds.Contains(questions[currentQuestionIndex].Id))
        {

            //Debug.LogError($"Question ID {questions[currentQuestionIndex].Id} has already been answered.");
            currentQuestionIndex++;
           // Debug.LogError($"Incremented currentQuestionIndex: {currentQuestionIndex}");
        }

        if (currentQuestionIndex < questions.Count)
        {
            Debug.Log($"Displaying question ID: {questions[currentQuestionIndex].Id}");
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
      
        hintAgent.StartQuestionTimer();

        questionText.text = question.QuestionText;
        if (optionButtons[0].GetComponentInChildren<TMP_Text>() != null &&
        optionButtons[1].GetComponentInChildren<TMP_Text>() != null &&
        optionButtons[2].GetComponentInChildren<TMP_Text>() != null &&
        optionButtons[3].GetComponentInChildren<TMP_Text>() != null)
        {
            optionButtons[0].GetComponentInChildren<TMP_Text>().text = question.AnswerOption1;
            optionButtons[1].GetComponentInChildren<TMP_Text>().text = question.AnswerOption2;
            optionButtons[2].GetComponentInChildren<TMP_Text>().text = question.AnswerOption3;
            optionButtons[3].GetComponentInChildren<TMP_Text>().text = question.AnswerOption4;
        }
        else
        {
            Debug.LogError("One or more buttons are missing a TMP_Text component.");
        } 
        foreach (Button button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        
        optionButtons[0].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption1, question.CorrectAnswer, question.Id));
        optionButtons[1].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption2, question.CorrectAnswer, question.Id));
        optionButtons[2].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption3, question.CorrectAnswer, question.Id));
        optionButtons[3].onClick.AddListener(() => OnAnswerSelected(question.AnswerOption4, question.CorrectAnswer, question.Id));
    }
    public void OnAnswerSelected(string selectedAnswer, string correctAnswer, int questionId)
    {
        float timeTaken = Time.time - questionStartTime;
        bool answeredCorrectly = selectedAnswer == correctAnswer;
        totalQuestionsAnswered++;
        float reward = 0.0f;
        if (answeredCorrectly)
        {
            if (AttemptsRemaining == maxAttemptsPerQuestion)  // Check if it's the first attempt
            {
                correctFirstAttemptCount++;  // Increment the count of first-time correct answers
            }
            playerScore += 10;
            UpdateScoreText();

            if (!answeredQuestionIds.Contains(questionId))
            {
                answeredQuestionIds.Add(questionId);
            }
            Debug.Log("Answered Question IDs: " + string.Join(", ", answeredQuestionIds));

            // Reward the agent based on time taken to answer
            reward = 1.0f - (timeTaken / maxAllowedTime);
            hintAgent.AddReward(reward);
            hintAgent.EndEpisode();  // End the episode

            if (currentQuestionIndex < questions.Count - 1)
            {
                currentQuestionIndex++;
                Debug.Log("Moving to next question. Current question index: " + currentQuestionIndex);
                hintText.text = "";
                //QuestionPanel2.SetActive(false);
                DisplayNextUnansweredQuestion();  // Display the next question

               // Debug.Log("Question transitioned, now ending the episode.");
            }
            else
            {
                Debug.Log("All questions answered.");
                EndGame();  // Handle game end if all questions are answered
            }
            Debug.Log("Ending the episode.");
          
            
        }
        else
        {
            // Penalize for incorrect answer
           playerScore -= 5;
           UpdateScoreText();
            // Penalize the agent for wrong answer
            hintAgent.AddReward(-0.1f);
            hintAgent.EndEpisode();  // End the episode
            Debug.Log("Incorrect answer.");
        }
    }

    public float GetPlayerSuccessRate()
    {
        if (totalQuestionsAnswered == 0) return 0.0f;  // Avoid division by zero
        return (float)correctFirstAttemptCount / totalQuestionsAnswered;
    }

    // Update the player's score on the UI
    public void UpdateScoreText()
    {
        scoreText.text ="Score"+playerScore.ToString();
        //Debug.LogError(playerScore);
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
        Debug.Log("All questions have been answered. Resetting for another round of training.");

        // Instead of ending the game, reset the questions and start over
        currentQuestionIndex = 0;
        answeredQuestionIds.Clear();  // Clear answered questions
        hintAgent.EndEpisode();  // Reset the agent

        // Re-enable the buttons for the new round
        foreach (Button button in optionButtons)
        {
            button.interactable = true;
        }

        // Restart the question loop
        DisplayNextUnansweredQuestion();

    }
    public string GetCorrectAnswer()
    {
        if (questions != null && currentQuestionIndex < questions.Count)
        {
            return questions[currentQuestionIndex].CorrectAnswer;  // Assuming each question has a 'CorrectAnswer' property
        }
        else
        {
            Debug.LogError("Unable to fetch the correct answer. Make sure questions are loaded and currentQuestionIndex is valid.");
            return string.Empty;  // Return an empty string in case of an error
        }
    }
    public string GetAnswerByIndex(int answerIndex)
    {
        if (currentQuestionIndex < questions.Count)
        {
            // Gather the answer options into an array
            string[] answerOptions = new string[]
            {
            questions[currentQuestionIndex].AnswerOption1,
            questions[currentQuestionIndex].AnswerOption2,
            questions[currentQuestionIndex].AnswerOption3,
            questions[currentQuestionIndex].AnswerOption4
            };

            if (answerIndex < answerOptions.Length)
            {
                return answerOptions[answerIndex];  // Return the selected answer based on the index
            }
            else
            {
                Debug.LogError("Invalid answer index selected.");
                return string.Empty;
            }
        }
        else
        {
            Debug.LogError("Question index is out of bounds.");
            return string.Empty;
        }
    }

    public int GetCurrentQuestionId()
    {
        return questions[currentQuestionIndex].Id;
    }

    public string GetAnswerByAction(int action)
    {
        if (questions != null && currentQuestionIndex < questions.Count)
        {
            QuestionModel currentQuestion = questions[currentQuestionIndex];
            switch (action)
            {
                case 0:
                    return currentQuestion.AnswerOption1;
                case 1:
                    return currentQuestion.AnswerOption2;
                case 2:
                    return currentQuestion.AnswerOption3;
                case 3:
                    return currentQuestion.AnswerOption4;
                default:
                    Debug.LogError("Invalid action for answer selection");
                    return null;
            }
        }
        return null;
    }


   

    public float GetAverageTimeSpent()
    {
        if (questionTimes.Count == 0)
        {
            return 0.0f;  // If no questions have been answered, return 0
        }

        float totalTime = 0.0f;
        foreach (float time in questionTimes)
        {
            totalTime += time;
        }

        return totalTime / questionTimes.Count;
    }
    public int GetAttemptsUsed()
    {
        return maxAttemptsPerQuestion - AttemptsRemaining;
    }

    public int GetRemainingQuestionsCount()
    {
        if (questions == null || answeredQuestionIds == null)
        {
            Debug.LogWarning("Questions or answeredQuestionIds not initialized.");
            return 0;  // Return 0 or an appropriate default value
        }

        return questions.Count - answeredQuestionIds.Count;
    }

    public float GetTotalTimeSpent()
    {
        float totalTime = 0.0f;
        foreach (float time in questionTimes)
        {
            totalTime += time;
        }

        return totalTime;
    }

}

