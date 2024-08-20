using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int PuzzleID { get; set; }
    public int Attempts { get; set; }
    public bool HintUsed { get; set; }

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

public enum HintAction
{
    NoHint,
    GeneralHint,
    SpecificHint,
    Solution
}
