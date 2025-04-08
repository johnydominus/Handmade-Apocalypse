using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName;
    public int tokens = 6;

    public bool hasPlayedOnce = false;
    public List<CardData> savedHand = new List<CardData>();

    public int[] investmentLevels = new int[3]; // for 2 spheres
    public int[] investmentTimers = new int[3]; // turns held

    public float[] emergencyLevels = new float[2];
    public bool[] isEmergencyActive = new bool[2];

    public int[,] incomingInvestments = new int[3, 2];  // [sphereIndex, investorIndex]
    public int[] dividendCounter = new int[3];          // Tracks turns until 3-turn payout
    public int[,] slowDividendCounters = new int[3, 2]; // [sphere, sphereOwnerIndex]


    public bool SpendToken(int amount)
    {
        if (tokens >= amount)
        {
            tokens -= amount;
            Debug.Log($"{playerName} spent a token. Tokens left: {tokens}");
            return true;
        }
        Debug.Log($"{playerName} has no tokens left.");
        return false;
    }

    public void RefillTokens()
    {
        tokens = 6;
        Debug.Log($"{playerName}'s tokens refilled.");
    }
}
