using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;  // Singleton pattern for easy access across scripts

    public int totalMinutes;  // Store minutes of the game
    public int totalSeconds;  // Store seconds of the game

    void Awake()
    {
        // Ensure that this script persists across scenes and there is only one instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Prevent this object from being destroyed when switching scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }
    }

    public void SetGameTime(int minutes, int seconds)
    {
        totalMinutes = minutes;
        totalSeconds = seconds;
    }

    public (int, int) GetGameTime()
    {
        return (totalMinutes, totalSeconds);
    }
}
