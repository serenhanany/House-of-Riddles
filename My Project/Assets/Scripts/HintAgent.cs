using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HintAgent : Agent
{
    private RLTrainingScript rlTrainingScript;

    public override void Initialize()
    {
        rlTrainingScript = GetComponent<RLTrainingScript>();
    }

    public override void OnEpisodeBegin()
    {
        StartCoroutine(WaitForQuestionsToLoad());
    }

    private IEnumerator WaitForQuestionsToLoad()
    {
        // Wait until questions are loaded
        while (rlTrainingScript.Questions == null || rlTrainingScript.Questions.Count == 0)
        {
           // Debug.Log("Waiting for questions to load...");
            yield return null; // Wait for the next frame
        }

        // Now, proceed to reset the environment
        rlTrainingScript.ResetEnvironment();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect the state space as observations
        var state = rlTrainingScript.GetState();
        foreach (var value in state.Values)
        {
            Debug.LogError(value);
            sensor.AddObservation(value);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int hintIndex = actions.DiscreteActions[0];  // The action determines which hint to provide

        // Declare hintLevel variable outside the if-else block
        int hintLevel;

        if (hintIndex >= 0 && hintIndex < rlTrainingScript.CurrentQuestion.Hints.Count)
        {
            var selectedHint = rlTrainingScript.CurrentQuestion.Hints[hintIndex];
            rlTrainingScript.GiveHint(selectedHint);  // This method is now properly accessible

            // Capture the hint level for use in reward calculation
            hintLevel = selectedHint.HintLevel;
        }
        else
        {
            // Default to a hint level of 0 if no valid hint is selected
            hintLevel = 0;
        }

        // Check the outcome and assign a reward
        bool answeredCorrectly = rlTrainingScript.CheckIfAnswerCorrect();
        float reward = rlTrainingScript.GetReward(answeredCorrectly, hintLevel);
        AddReward(reward);

        if (rlTrainingScript.IsEpisodeDone())
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Implement if you want to manually control the agent during training/testing
    }
}
