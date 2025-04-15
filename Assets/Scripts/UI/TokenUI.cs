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
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
        GameEvents.OnTokenSpent.UnregisterListener(OnTokenSpent);
        GameEvents.OnCardPlayedWithOwner.UnregisterListener(OnCardPlayedWithOwner);
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

    public void UpdateDisplay(PlayerController player)
    {
        if (tokenText == null || player == null)
        {
            Debug.LogWarning("TokenUI not fully initialized!");
            return;
        }

        tokenText.text = $"Tokens: {player.tokenManager.GetTokens()}";
        Debug.Log($"TokenUI updated for {player.playerName}: {player.tokenManager.GetTokens()} tokens.");
    }
}
