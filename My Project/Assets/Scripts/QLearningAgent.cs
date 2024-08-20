using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearningAgent : MonoBehaviour
{
    private Dictionary<(GameState, HintAction), float> qTable;
    private float learningRate;
    private float discountFactor;

    public QLearningAgent(float learningRate = 0.1f, float discountFactor = 0.9f)
    {
        qTable = new Dictionary<(GameState, HintAction), float>();
        this.learningRate = learningRate;
        this.discountFactor = discountFactor;
    }

    // Initialize Q-values for a state-action pair
    public void InitializeState(GameState state, HintAction[] actions)
    {
        foreach (var action in actions)
        {
            if (!qTable.ContainsKey((state, action)))
            {
                qTable[(state, action)] = 0f; // Initialize Q-value to 0
            }
        }
    }

    // Get the best action based on the current state
    public HintAction GetBestAction(GameState state, HintAction[] actions)
    {
        InitializeState(state, actions);

        HintAction bestAction = actions[0];
        float maxQValue = float.MinValue;

        foreach (var action in actions)
        {
            float qValue = qTable[(state, action)];
            if (qValue > maxQValue)
            {
                maxQValue = qValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    // Update the Q-value for a state-action pair
    public void UpdateQValue(GameState state, HintAction action, float reward, GameState nextState)
    {
        InitializeState(nextState, new HintAction[] { HintAction.NoHint, HintAction.GeneralHint, HintAction.SpecificHint, HintAction.Solution });

        float currentQ = qTable[(state, action)];
        float maxQNext = GetMaxQValue(nextState, new HintAction[] { HintAction.NoHint, HintAction.GeneralHint, HintAction.SpecificHint, HintAction.Solution });

        // Q-Learning formula
        qTable[(state, action)] = currentQ + learningRate * (reward + discountFactor * maxQNext - currentQ);
    }

    private float GetMaxQValue(GameState state, HintAction[] actions)
    {
        float maxQValue = float.MinValue;
        foreach (var action in actions)
        {
            float qValue = qTable[(state, action)];
            if (qValue > maxQValue)
            {
                maxQValue = qValue;
            }
        }
        return maxQValue;
    }
}

public class RewardSystem
{
    public float GetReward(bool puzzleSolved, int attemptsAfterHint)
    {
        if (puzzleSolved)
        {
            return 10f - attemptsAfterHint; // Reward decreases with more attempts
        }
        return -5f; // Negative reward for not solving the puzzle
    }
}
