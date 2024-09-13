using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;





public class HintAgent : Agent
{
    public TMP_Text hintText;
    public FetchQuestions fetchQuestions; // Reference to the FetchQuestions class
    private QuestionModel currentQuestion;
    public int playerScore;
    private bool hintGiven;
    private float questionStartTime;
    //private int attemptsRemaining = 4;

    public override void OnEpisodeBegin()
    {
        if (!fetchQuestions.questionsLoaded || !HintsAreLoaded())
        {
            Debug.LogWarning("Questions or hints are not yet loaded. Waiting to start episode...");
            this.enabled = false;
            return;  // Do not start the episode until questions and hints are fully loaded
        }

        currentQuestion = fetchQuestions.GetCurrentQuestion();
        hintGiven = false;
        questionStartTime = Time.time;
        Debug.Log($"Starting new episode with question: {currentQuestion.QuestionText}");
    }

    private bool HintsAreLoaded()
    {
        foreach (var question in fetchQuestions.Questions)
        {
            if (question.Hints == null || question.Hints.Count < 3)
            {
                Debug.LogError($"Hints not fully loaded for question ID: {question.Id}");
                return false;
            }
        }
        return true;
    }



    public void StartQuestionTimer()
    {
        questionStartTime = Time.time;  // Set the start time when the question is presented
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe player's score and whether a hint was given
        sensor.AddObservation(playerScore);
        sensor.AddObservation(hintGiven ? 1.0f : 0.0f);
        float timeSpent = Time.time - questionStartTime;
        sensor.AddObservation(timeSpent);
    }
    
    
    private int attemptsRemaining = 4;  // Number of attempts allowed per question

    /*public override void OnActionReceived(ActionBuffers actions)
    {
        if (!fetchQuestions.questionsLoaded)
        {
            Debug.LogError("Questions are not loaded yet. Cannot give a hint.");
            return;
        }

        int hintAction = actions.DiscreteActions[0];  // The agent selects a hint level
        int answerAction = actions.DiscreteActions[1];  // The agent selects an answer

        float baseReward = 3.0f;  // Start with a high reward for no hints

        // Action 0 = Easy hint, Action 1 = Medium hint, Action 2 = Hard hint
        switch (hintAction)
        {
            case 0:
                fetchQuestions.GiveHint(1);  // Easy hint (level 1)
                baseReward -= 0.5f;  // Small deduction for an easy hint
                Debug.Log("Easy hint given.");
                break;
            case 1:
                fetchQuestions.GiveHint(2);  // Medium hint (level 2)
                baseReward -= 1.0f;  // Medium deduction for a medium hint
                Debug.Log("Medium hint given.");
                break;
            case 2:
                fetchQuestions.GiveHint(3);  // Hard hint (level 3)
                baseReward -= 1.5f;  // Largest deduction for a hard hint
                Debug.Log("Hard hint given.");
                break;
        }

        // Agent selects the answer (answerAction maps to one of the answer options)
        string selectedAnswer = fetchQuestions.GetAnswerByAction(answerAction);

        // Check if the selected answer is correct
        bool answeredCorrectly = fetchQuestions.CheckIfAnswerCorrect(selectedAnswer);

        float timeTaken = Time.time - fetchQuestions.questionStartTime;
        float timePenalty = Mathf.Clamp(1.0f / timeTaken, 0.1f, 1.0f);  // Adjust reward based on time taken
        string correctAnswer = fetchQuestions.GetCorrectAnswer();
        Debug.Log($"Selected Answer: {selectedAnswer}, Correct Answer: {correctAnswer}");

        if (answeredCorrectly)
        {
            // Reward based on hint level and time taken
            AddReward(baseReward * timePenalty);
            Debug.Log($"Correct answer! Base reward: {baseReward}, Time penalty applied: {timePenalty}");

            // Reset attempts and move to the next question
            fetchQuestions.ResetAttempts();
            fetchQuestions.DisplayNextUnansweredQuestion();
            questionStartTime = Time.time;  // Reset the question start time for the next question
        }
        else
        {
            // Penalty for incorrect answer
            AddReward(-1.5f);
            Debug.Log("Incorrect answer. Penalty applied.");

            // Allow another try if attempts are left
            if (fetchQuestions.AttemptsRemaining > 0)
            {
                fetchQuestions.AttemptsRemaining--;
                Debug.Log($"Attempts remaining: {fetchQuestions.AttemptsRemaining}. Allowing another try.");
            }
            else
            {
                // If no attempts remain, reset attempts and move to the next question
                fetchQuestions.ResetAttempts();
                fetchQuestions.DisplayNextUnansweredQuestion();
                questionStartTime = Time.time;  // Reset the question start time for the next question
            }
        }

        EndEpisode();  // End the episode after the answer
    }*/
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!fetchQuestions.questionsLoaded)
        {
            Debug.LogError("Questions are not loaded yet. Cannot give a hint.");
            return;
        }

        int hintAction = actions.DiscreteActions[0];  // The agent selects a hint level
        int answerAction = actions.DiscreteActions[1];  // The agent selects an answer
       // Debug.LogError("hintAction:"+actions.DiscreteActions[0]);
        //Debug.LogError("answerAction"+actions.DiscreteActions[1]);
        float baseReward = 5.0f;  // Start with a high reward for no hints

        // Action 0 = Easy hint, Action 1 = Medium hint, Action 2 = Hard hint
        switch (hintAction)
        {
            case 0:
                fetchQuestions.GiveHint(1);  // Easy hint (level 1)
                baseReward -= 0.5f;  // Small deduction for an easy hint
                Debug.Log("Easy hint given.");
                break;
            case 1:
                fetchQuestions.GiveHint(2);  // Medium hint (level 2)
                baseReward -= 1.0f;  // Medium deduction for a medium hint
                Debug.Log("Medium hint given.");
                break;
            case 2:
                fetchQuestions.GiveHint(3);  // Hard hint (level 3)
                baseReward -= 1.5f;  // Largest deduction for a hard hint
                Debug.Log("Hard hint given.");
                break;
        }

        // Agent selects the answer (answerAction maps to one of the answer options)
        string selectedAnswer = fetchQuestions.GetAnswerByAction(answerAction);

        // Check if the selected answer is correct
        bool answeredCorrectly = fetchQuestions.CheckIfAnswerCorrect(selectedAnswer);

        float timeTaken = Time.time - fetchQuestions.questionStartTime;
        float timePenalty = Mathf.Clamp(1.0f / timeTaken, 0.1f, 1.0f);  // Adjust reward based on time taken
        string correctAnswer = fetchQuestions.GetCorrectAnswer();
        //Debug.Log($"Selected Answer: {selectedAnswer}, Correct Answer: {correctAnswer}");

        // Simulate OnAnswerSelected logic here
        if (answeredCorrectly)
        {
            // Reward based on hint level and time taken
            AddReward(baseReward * timePenalty);
            Debug.Log($"Correct answer! Base reward: {baseReward}, Time penalty applied: {timePenalty}");

            // Add the current question to the answered question list
            //fetchQuestions.answeredQuestionIds.Add(fetchQuestions.currentQuestion.Id);

            // Move to the next question before calling EndEpisode
            fetchQuestions.playerScore += 10;
            fetchQuestions.UpdateScoreText();
            fetchQuestions.DisplayNextUnansweredQuestion();
        }
        else
        {
            // Penalty for incorrect answer
            AddReward(-0.1f);
            Debug.Log("Incorrect answer. Penalty applied.");

            // Retry logic (if retries are allowed)
            if (fetchQuestions.AttemptsRemaining > 0)
            {
                fetchQuestions.AttemptsRemaining--;
                Debug.Log($"Attempts remaining: {fetchQuestions.AttemptsRemaining}. Allowing another try.");
                // Do not end the episode, let the agent retry
            }
            else
            {
                // No retries left, end the episode
                fetchQuestions.DisplayNextUnansweredQuestion();
                EndEpisode();
            }
        }
    }













    // Function to provide hints based on hint level (easy = 1, medium = 2, hard = 3)
    private void GiveHint(int hintLevel)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("No current question available.");
            return;
        }

        // Select the hint based on the hint level (easy, medium, hard)
        HintModel selectedHint = currentQuestion.Hints.Find(h => h.HintLevel == hintLevel);
        if (selectedHint != null)
        {
            hintText.text = selectedHint.HintText; // Display the hint in the UI
            hintGiven = true;
            AddReward(-0.1f); // Small penalty for giving a hint to encourage minimal hints
        }
        else
        {
            Debug.LogWarning("No hint available for the specified level.");
        }
    }
}



