using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLTrainingScript : MonoBehaviour
{
    public int playerLevel;
    public int playerScore;
    public int currentQuestionId;
    public int attempts;
    public bool hintGiven;
    public float timeSpentOnQuestion;

    // Action Space
    public enum RLAction
    {
        GiveHint,
        SkipQuestion,
        AdjustDifficulty
    }

    // State Space
    public Dictionary<string, float> GetState()
    {
        return new Dictionary<string, float>
        {
            { "PlayerLevel", playerLevel },
            { "PlayerScore", playerScore },
            { "CurrentQuestionId", currentQuestionId },
            { "Attempts", attempts },
            { "HintGiven", hintGiven ? 1.0f : 0.0f },
            { "TimeSpentOnQuestion", timeSpentOnQuestion }
        };
    }

    // Reward Function
    public float GetReward(bool answeredCorrectly)
    {
        if (answeredCorrectly && !hintGiven)
        {
            return 10.0f; // High reward for correct answer without hint
        }
        else if (answeredCorrectly && hintGiven)
        {
            return 5.0f; // Smaller reward for correct answer with hint
        }
        else
        {
            return -10.0f; // Penalty for incorrect answer
        }
    }

    // Example of taking an action
    public void TakeAction(RLAction action)
    {
        switch (action)
        {
            case RLAction.GiveHint:
                // Logic to give a hint
                break;
            case RLAction.SkipQuestion:
                // Logic to skip to the next question
                break;
            case RLAction.AdjustDifficulty:
                // Logic to adjust the difficulty
                break;
        }
    }

}
