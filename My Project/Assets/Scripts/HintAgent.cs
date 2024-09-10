using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;
using System.Diagnostics;






public class HintAgent : Agent
{
    public TMP_Text hintText;
    public FetchQuestions fetchQuestions; // Reference to the FetchQuestions class
    private QuestionModel currentQuestion;
    public int playerScore;
    private bool hintGiven;
    private float questionStartTime;

    public override void OnEpisodeBegin()
    {
        if (fetchQuestions == null)
        {
            Debug.LogError("FetchQuestions reference is not assigned in HintAgent.");
            return;
        }

        if (!fetchQuestions.questionsLoaded)
        {
            Debug.LogWarning("Questions are not yet loaded. Waiting to start episode...");
           this.enabled = false;
            return; // Do not start the episode until questions are loaded
        }

        hintGiven = false;
        currentQuestion = fetchQuestions.GetCurrentQuestion(); // Get the current question from FetchQuestions
        questionStartTime = Time.time;
        if (currentQuestion == null)
        {
            Debug.LogError("No current question available.");
            return;
        }
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
    /*
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!fetchQuestions.questionsLoaded)
        {
            Debug.LogError("Questions are not loaded yet. Cannot give a hint.");
           // EndEpisode();
            return;
        }
        int action = actions.DiscreteActions[0];

        // Action 0 = Easy hint, Action 1 = Medium hint, Action 2 = Hard hint
        switch (action)
        {
            case 0:
                fetchQuestions.GiveHint(1); // Easy hint (level 1)
                break;
            case 1:
                fetchQuestions.GiveHint(2); // Medium hint (level 2)
                break;
            case 2:
                fetchQuestions.GiveHint(3); // Hard hint (level 3)
                break;
        }

        string selectedAnswer = fetchQuestions.GetSelectedAnswer();
        bool answeredCorrectly = fetchQuestions.CheckIfAnswerCorrect(selectedAnswer);

        if (answeredCorrectly && !hintGiven)
        {
            AddReward(2.0f);  // Greater reward if no hint was given and correct answer
        }
        else if (answeredCorrectly)
        {
            AddReward(1.0f);  // Regular reward for a correct answer with hints
        }
        else
        {
            AddReward(-1.0f);  // Penalty for incorrect answer
        }

        EndEpisode(); // End the episode after a decision is made
    }*/
    /* public override void OnActionReceived(ActionBuffers actions)
     {
         if (!fetchQuestions.questionsLoaded)
         {
             Debug.LogError("Questions are not loaded yet. Cannot give a hint.");
             return;
         }
         string selectedAnswer = fetchQuestions.GetSelectedAnswer();
         if (string.IsNullOrEmpty(selectedAnswer))
         {
             Debug.LogWarning("No answer selected yet. Waiting for player to select an answer.");
             return;  // Wait for the player to select an answer
         }

         int action = actions.DiscreteActions[0];  // The agent selects a hint level
         float baseReward = 3.0f;  // Start with a high reward for no hints

         // Action 0 = Easy hint, Action 1 = Medium hint, Action 2 = Hard hint
         switch (action)
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

         //string selectedAnswer = fetchQuestions.GetSelectedAnswer();
         bool answeredCorrectly = fetchQuestions.CheckIfAnswerCorrect(selectedAnswer);
         string correctAnswer = fetchQuestions.GetCorrectAnswer();
         // Track time taken to answer the question
         Debug.Log($"Selected Answer: {selectedAnswer}, Correct Answer: {correctAnswer}");
         float timeTaken = Time.time - fetchQuestions.questionStartTime;
         float timePenalty = Mathf.Clamp(1.0f / timeTaken, 0.1f, 1.0f);  // Adjust reward based on time taken

         if (answeredCorrectly)
         {
             // Reward based on hint level and time taken
             AddReward(baseReward * timePenalty);
             Debug.Log($"Correct answer! Base reward: {baseReward}, Time penalty applied: {timePenalty}");
         }
         else
         {
             // Penalty for incorrect answer
             AddReward(-1.5f);
             Debug.Log("Incorrect answer. Penalty applied.");
         }

         EndEpisode();  // End the episode after a decision is made
     }*/
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!fetchQuestions.questionsLoaded)
        {
            Debug.LogError("Questions are not loaded yet. Cannot give a hint.");
            return;
        }

        // Action 0 = hint level, Action 1 = answer selection (0 to number of answers - 1)
        int hintAction = actions.DiscreteActions[0];  // The agent selects a hint level
        int answerAction = actions.DiscreteActions[1];  // The agent selects an answer

        float baseReward = 3.0f;  // Start with a high reward for no hints

        // Give a hint based on the agent's selected hint level
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

        // Simulate answer selection by the agent
        string selectedAnswer = fetchQuestions.GetAnswerByIndex(answerAction);  // Agent selects an answer by index
        string correctAnswer = fetchQuestions.GetCorrectAnswer();
        bool answeredCorrectly = fetchQuestions.CheckIfAnswerCorrect(selectedAnswer);

        Debug.Log($"Selected Answer: {selectedAnswer}, Correct Answer: {correctAnswer}");

        // Track time taken to answer the question
        float timeTaken = Time.time - fetchQuestions.questionStartTime;
        float timePenalty = Mathf.Clamp(1.0f / timeTaken, 0.1f, 1.0f);  // Adjust reward based on time taken

        if (answeredCorrectly)
        {
            // Reward based on hint level and time taken
            AddReward(baseReward * timePenalty);
            Debug.Log($"Correct answer! Base reward: {baseReward}, Time penalty applied: {timePenalty}");
        }
        else
        {
            // Penalty for incorrect answer
            AddReward(-1.5f);
            Debug.Log("Incorrect answer. Penalty applied.");
        }

        EndEpisode();  // End the episode after a decision is made
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



