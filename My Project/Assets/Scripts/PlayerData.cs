using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public int playerId { get; private set; }
    public string playerName { get; private set; }
    public int playerLevel { get; private set; }
    public int Score { get; set; }
    public int CurrentQuestionAttempts { get; set; }
    public bool HintGiven { get; set; }
    public float TimeSpentOnCurrentQuestion { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerData(int id, string name, int level)
    {
        playerId = id;
        playerName = name;
        playerLevel = level;
        ResetQuestionData();  // Initialize or reset question-related data
    }

    public void ResetQuestionData()
    {
        CurrentQuestionAttempts = 0;
        HintGiven = false;
        TimeSpentOnCurrentQuestion = 0f;
    }

    public void ClearPlayerData()
    {
        playerId = 0;
        playerName = string.Empty;
        playerLevel = 0;
        Score = 0;
        ResetQuestionData();  // Reset question-related data as well
        Debug.Log("Player data cleared.");
    }
}
