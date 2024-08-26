using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public int playerId { get; private set; }
    public string playerName { get; private set; }
    public int playerLevel { get; private set; }

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
    }
}

