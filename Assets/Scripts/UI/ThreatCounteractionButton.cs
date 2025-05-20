using UnityEngine;
using UnityEngine.UI;

public class ThreatCounteractionButton : MonoBehaviour
{
    [SerializeField] private Button counteractButton;
    [SerializeField] private ThreatType threatType;

    private void Awake()
    {
        // Ensure we have a reference for the button
        if (counteractButton == null)
            counteractButton = GetComponent<Button>();

        // Add click listener
        counteractButton.onClick.AddListener(OnCounteractButtonClicked);
    }

    private void OnEnable()
    {
        GameEvents.OnTokenSpent.RegisterListener(OnTokenSpent);
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTokenSpent.UnregisterListener(OnTokenSpent);
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);

        // Remove click listener
        if (counteractButton != null)
            counteractButton.onClick.RemoveListener(OnCounteractButtonClicked);
    }

    private void OnTokenSpent(PlayerController player)
    {
        // Update button interactibility whenever tokens are spent
        UpdateButtonInteractibility();
    }

    private void OnTurnStarted(TurnContext context)
    {
        // Update button interactibility whenever a turn starts
        UpdateButtonInteractibility();
    }

    private void UpdateButtonInteractibility()
    {
        if (this.isActiveAndEnabled)
        {
            // Only enable button if current player has tokens
            PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();
            bool hasTokens = currentPlayer != null && currentPlayer.tokenManager.GetTokens() > 0;

            // Also disable for Asteroid type as it has special handling
            bool isAsteroid = threatType == ThreatType.Asteroid;

            counteractButton.interactable = hasTokens && !isAsteroid;
        }
    }

    public void OnCounteractButtonClicked()
    {
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();

        // Attempt to spend a token 
        if (currentPlayer.SpendToken(1))
        {
            // If successful, decrease the threat by 1
            GameServices.Instance.threatManager.ApplyThreatChange(threatType, -1);

            Debug.Log($"Player {currentPlayer.playerName} spent 1 token to reduce {threatType} threat by 1");
        }
        else
        {
            Debug.Log($"Player {currentPlayer.playerName} doesn't have enough tokens");
        }
    }

    // Public method to set the threat type (can be called from editor or code)
    public void SetThreatType(ThreatType type)
    {
        threatType = type;

        // Update immediately in case conditions changed
        UpdateButtonInteractibility();
    }
}
