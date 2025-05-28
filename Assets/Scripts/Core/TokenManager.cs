using UnityEngine;

public class TokenManager
{
    private int tokens;
    private PlayerController player;

    public void Initialize(int startingTokens, PlayerController thePlayer)
    {
        tokens = startingTokens;
        player = thePlayer;
    }

    public bool HasEnoughTokens(int amount) => tokens >= amount;

    public int GetTokens() => tokens;

    public void SetTokens(int amount) 
    {
        tokens = amount;    // Remove any clamping to allow negetives
        Debug.Log($"{player.playerName} tokens set to {tokens}");
    }

    public void SpendTokens(int amount)
    {
        tokens -= amount;
        GameEvents.OnTokenSpent.Raise(player);
        GameEvents.OnTokensChanged.Raise(player);
        Debug.Log($"{player.playerName} spent {amount} tokens. New balance: {tokens}");
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
        GameEvents.OnTokensChanged.Raise(player);
        Debug.Log($"{player.playerName} received {amount} tokens. New total: {tokens}");
    }
}
