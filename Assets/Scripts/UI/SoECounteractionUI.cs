using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SoECounteractionUI : MonoBehaviour
{
    [SerializeField] private Button addTokenButton;
    [SerializeField] private TextMeshProUGUI counterText;

    private EmergencyType? currentEmergency;
    private PlayerController currentPlayer;

    private void OnEnable()
    {
        GameEvents.OnTokenSpent.RegisterListener(OnTokenSpent);
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTokenSpent.UnregisterListener(OnTokenSpent);
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    public void Activate(SoEContext context)
    {
        currentEmergency = context.soeType;
        currentPlayer = context.player;

        Debug.Log($"SoEActivated: {currentEmergency} for {currentPlayer.playerName}");

        if (currentEmergency != null && currentPlayer != null)
        {
            // Find the emergency in the player's list
            var emergency = currentPlayer.emergencies.Find(e => e.emergencyType == currentEmergency);
            if (emergency != null && emergency.stateOfEmergency.isActive)
            {
                ShowCounteractionUI(emergency);
            }
        }
    }


    private void OnTurnStarted(TurnContext context)
    {
        if (context.player == currentPlayer && currentEmergency != null)
        {
            // Update the UI if this player has an active SoE
            var emergency = currentPlayer.emergencies.Find(e => e.emergencyType == currentEmergency);
            if (emergency != null && emergency.stateOfEmergency.isActive)
            {
                UpdateCounterUI(emergency.stateOfEmergency.tokensPut);
            }
            else
            {
                HideCounteractionUI();
            }
        }
    }

    private void OnTokenSpent(PlayerController player)
    {
        if (player == currentPlayer && currentEmergency != null)
        {
            // Update the UI if this player spent tokens on the SoE
            var emergency = currentPlayer.emergencies.Find(e => e.emergencyType == currentEmergency);
            if (emergency != null && emergency.stateOfEmergency.isActive)
            {
                UpdateCounterUI(emergency.stateOfEmergency.tokensPut);
            }
            else
            {
                HideCounteractionUI();
            }
        }
    }

    public void OnAddTokenClicked()
    {
        Debug.Log("Add Token button clicked!");
        Debug.Log($"Current player is {(currentPlayer == null ? "null" : currentPlayer.playerName)}\n" +
                  $"Current emergency is {(currentEmergency == null ? "null" : currentEmergency.ToString())}");
        if (currentPlayer != null && currentEmergency != null)
        {
            Debug.Log("Executing the Add Token logic...");

            // Add a token to counteract the SoE
            GameServices.Instance.soeManager.SpendTokensToReduce(currentPlayer, currentEmergency, 1);

            // Check if SoE is still active after adding the token
            var emergency = currentPlayer.emergencies.Find(e => e.emergencyType == currentEmergency);
            if (emergency != null)
            {
                if (!emergency.stateOfEmergency.isActive)
                {
                    HideCounteractionUI();
                }
                else
                {
                    UpdateCounterUI(emergency.stateOfEmergency.tokensPut);
                    Debug.Log($"SoE is still active. Tokens put: {emergency.stateOfEmergency.tokensPut}");
                }
            }
        }
    }

    private void ShowCounteractionUI(Emergency emergency)
    {
        gameObject.SetActive(true);
        UpdateCounterUI(emergency.stateOfEmergency.tokensPut);

        // Update button interactability based on whether player has tokens
        addTokenButton.interactable = currentPlayer.tokenManager.GetTokens() > 0;
    }

    private void UpdateCounterUI(int tokenCount)
    {
        counterText.text = $"{tokenCount}";

        // Update button interactability
        addTokenButton.interactable = currentPlayer.tokenManager.GetTokens() > 0;

        // If tokens reach 12, the SoE should auto-deactivate via SoE logic
    }

    private void HideCounteractionUI()
    {
        gameObject.SetActive(false);
        currentEmergency = null;
        currentPlayer = null;
    }
}