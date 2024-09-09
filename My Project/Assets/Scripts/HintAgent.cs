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

        if (currentQuestion == null)
        {
            Debug.LogError("No current question available.");
            return;
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe player's score and whether a hint was given
        sensor.AddObservation(playerScore);
        sensor.AddObservation(hintGiven ? 1.0f : 0.0f);
    }

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



