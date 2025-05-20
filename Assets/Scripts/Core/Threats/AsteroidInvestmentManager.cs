using System.Collections.Generic;
using UnityEngine;

public class AsteroidInvestmentManager : MonoBehaviour
{
    private static AsteroidInvestmentManager _instance;
    public static AsteroidInvestmentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AsteroidInvestmentManager");
                _instance = go.AddComponent<AsteroidInvestmentManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Store investments by player
    private Dictionary<string, int> playerInvestments = new();
    public int investmentCost = 50;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Get a player's investment
    public int GetPlayerInvestment(PlayerController player)
    {
        if (playerInvestments.TryGetValue(player.playerName, out int investment))
        {
            return investment;
        }
        return 0;
    }

    // Add investment for a player
    public void AddInvestment(PlayerController player, int amount)
    {
        if (!playerInvestments.ContainsKey(player.playerName))
        {
            playerInvestments[player.playerName] = 0;
        }

        playerInvestments[player.playerName] += amount;
        Debug.Log($"Added {amount} investment for {player.playerName}. Total: {playerInvestments[player.playerName]}");
    }

    // Get total investment
    public int GetTotalInvestment()
    {
        int total = 0;
        foreach (var investment in playerInvestments.Values)
        {
            total += investment;
        }
        return total;
    }

    // Check if asteroid defense is complete
    public bool IsDefenseComplete()
    {
        int playerCount = GameServices.Instance.turnManager.GetAllPlayers().Count;
        int requiredTotal = playerCount * investmentCost;

        return GetTotalInvestment() >= requiredTotal;
    }

    // Clear all investments (e.g. when asteroid is defeated)
    public void ClearInvestments()
    {
        playerInvestments.Clear();
        Debug.Log("All investments cleared.");
    }
}
