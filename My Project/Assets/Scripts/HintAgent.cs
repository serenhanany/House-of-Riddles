using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HintAgent : Agent
{
    public override void OnEpisodeBegin()
    {
        // Reset environment at the start of each episode
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect game state as observations
        sensor.AddObservation(PlayerData.Instance.CurrentQuestionAttempts);
        sensor.AddObservation(PlayerData.Instance.HintGiven);
        sensor.AddObservation(PlayerData.Instance.TimeSpentOnCurrentQuestion);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        if (action == 0)
        {
            // No hint given
        }
        else if (action == 1)
        {
            // Give hint
        }

        // Define rewards
        AddReward(1.0f);  // Example: Positive reward for correct answer
    }
}
