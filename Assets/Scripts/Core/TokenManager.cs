using UnityEngine;

public class TokenManager
{
    private int tokens;

    public void Initialize(int startingTokens)
    {
        tokens = startingTokens;
    }

    public bool HasEnoughTokens(int amount) => tokens >= amount;

    public void SpendTokens(int amount)
    {
        tokens = Mathf.Max(0, tokens - amount);
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
    }

    public int GetAvailableTokens() => tokens;
}
