using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Class representing the game state for a specific puzzle
public class GameState
{
    // Properties of the GameState class
    public int PuzzleID { get; set; }
    public int Attempts { get; set; }
    public bool HintUsed { get; set; }
    // Constructor to initialize a new GameState object
    public GameState(int puzzleID, int attempts, bool hintUsed)
    {
        PuzzleID = puzzleID;
        Attempts = attempts;
        HintUsed = hintUsed;
    }

    // Override Equals and GetHashCode for proper comparison
    public override bool Equals(object obj)
    {
        if (obj is GameState)
        {
            GameState other = (GameState)obj;
            return PuzzleID == other.PuzzleID && Attempts == other.Attempts && HintUsed == other.HintUsed;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (PuzzleID, Attempts, HintUsed).GetHashCode();
    }
}
// Enumeration representing the different hint actions a player can take
public enum HintAction
{
    NoHint,
    GeneralHint,
    SpecificHint,
    Solution
}
