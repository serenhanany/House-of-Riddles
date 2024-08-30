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
        // Reset the environment or agent state at the beginning of an episode
        rlTrainingScript.ResetEnvironment();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect the state space as observations
        var state = rlTrainingScript.GetState();
        foreach (var value in state.Values)
        {
            sensor.AddObservation(value);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Convert the discrete action into the RLAction enum
        RLTrainingScript.RLAction action = (RLTrainingScript.RLAction)actions.DiscreteActions[0];

        // Execute the action in the environment
        rlTrainingScript.TakeAction(action);

        // Get the reward based on the action outcome
        bool answeredCorrectly = rlTrainingScript.CheckIfAnswerCorrect();
        float reward = rlTrainingScript.GetReward(answeredCorrectly);
        AddReward(reward);

        // Check if the episode should end
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
