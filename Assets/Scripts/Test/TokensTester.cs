using TMPro;
using UnityEngine;

public class TokensTester : MonoBehaviour
{
    private PlayerController player;
    private int tokens;
    public TextMeshProUGUI tokensText;

    // Start is called before the first frame update
    void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
        GameEvents.OnTokensChanged.RegisterListener(OnTokensChanged);
    }

    void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    void OnTurnStarted(TurnContext turnContext)
    {
        player = GameServices.Instance.turnManager.GetCurrentPlayer();
        tokens = player.tokenManager.GetTokens();
    }

    void OnTokensChanged(PlayerController player)
    {
        tokens = player.tokenManager.GetTokens();
        tokensText.text = $"Tokens: {tokens}";
    }

    public void AddTokens()
    {
        player = GameServices.Instance.turnManager.GetCurrentPlayer();
        player.tokenManager.AddTokens(1);
    }

    public void SpendTokens()
    {
        player = GameServices.Instance.turnManager.GetCurrentPlayer();
        player.tokenManager.SpendTokens(1);
    }
}
