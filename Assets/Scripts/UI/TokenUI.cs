using UnityEngine;
using TMPro;

public class TokenUI : MonoBehaviour
{
    public TextMeshProUGUI tokenText;

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
        GameEvents.OnTokenSpent.RegisterListener(OnTokenSpent);
        GameEvents.OnCardPlayedWithOwner.RegisterListener(OnCardPlayedWithOwner);
        GameEvents.OnTokensChanged.RegisterListener(OnTokensChanged);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
        GameEvents.OnTokenSpent.UnregisterListener(OnTokenSpent);
        GameEvents.OnCardPlayedWithOwner.UnregisterListener(OnCardPlayedWithOwner);
        GameEvents.OnTokensChanged.UnregisterListener(OnTokensChanged);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        UpdateDisplay(turnContext.player);
    }

    private void OnTokenSpent(PlayerController player)
    {
        UpdateDisplay(player);
        Debug.Log("Token must be spent!");
    }

    private void OnCardPlayedWithOwner(CardPlayContext context)
    {
        UpdateDisplay(context.player);
    }

    private void OnTokensChanged(PlayerController player)
    {
        UpdateDisplay(player);
    }

    public void UpdateDisplay(PlayerController player)
    {
        if (tokenText == null || player == null)
        {
            Debug.LogWarning("TokenUI not fully initialized!");
            return;
        }

        int tokens = player.tokenManager.GetTokens();

        // Color coding for negative balances
        if (tokens < 0)
            tokenText.text = $"Tokens: <color=red>{tokens}</color>";
        else
            tokenText.text = $"Tokens: {tokens}";

        Debug.Log($"TokenUI updated for {player.playerName}: {player.tokenManager.GetTokens()} tokens.");
    }
}
