using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml.Serialization;

public class DelayedEffectCounteractionPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button contributeButton;

    [Header("Display Elements")]
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI turnsRemainingText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI consequencesText;

    [Header("Contribution Controls")]
    [SerializeField] private Button contribute1Button;
    [SerializeField] private Button contribute5Button;
    [SerializeField] private Button contributeAllButton;
    [SerializeField] private TextMeshProUGUI currentPlayerTokensText;

    [Header("Player Contributions")]
    [SerializeField] private Transform playerContributionsContainer;
    [SerializeField] private GameObject playerContributionPrefab;

    private DelayedCounterableEffect currentEffect;
    private bool isProcessingContribution = false;

    private void Awake()
    {
        // Hide panel initially
        panelRoot.SetActive(false);

        // Set up buttons listeners
        closeButton.onClick.AddListener(ClosePanel);
        contributeButton.onClick.AddListener(() => ContributeTokens(1));
        contribute1Button.onClick.AddListener(() => ContributeTokens(1));
        contribute5Button.onClick.AddListener(() => ContributeTokens(5));
        contributeAllButton.onClick.AddListener(ContributeAllTokens);

        Debug.Log("DelayedEffectCounteractionPanel initialized.");
    }

    private void OnEnable()
    {
        GameEvents.OnDelayedEffectUpdated.RegisterListener(OnDelayedEffectUpdated);
        GameEvents.OnDelayedEffectCountered.RegisterListener(OnDelayedEffectCountered);
        GameEvents.OnTokensChanged.RegisterListener(OnTokensChanged);
    }

    private void OnDisable()
    {
        GameEvents.OnDelayedEffectUpdated.UnregisterListener(OnDelayedEffectUpdated);
        GameEvents.OnDelayedEffectCountered.UnregisterListener(OnDelayedEffectCountered);
        GameEvents.OnTokensChanged.UnregisterListener(OnTokensChanged);
    }

    public void OpenPanel(DelayedCounterableEffect effect)
    {
        currentEffect = effect;
        panelRoot.SetActive(true);
        UpdateDisplay();
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
        currentEffect = null;
    }

    private void UpdateDisplay()
    {
        if (currentEffect == null) return;

        // Update main info
        effectNameText.text = currentEffect.description;

        if (currentEffect.turnsRemaining > 1)
            turnsRemainingText.text = $"{currentEffect.turnsRemaining} turns remaining until disaster!";
        else if (currentEffect.turnsRemaining == 1)
            turnsRemainingText.text = "FINAL TURN - Disaster strikes next turn!";
        else
            turnsRemainingText.text = "Disaster is happening NOW!";

        // Update progress
        float progress = (float)currentEffect.tokensContributed / currentEffect.tokensRequired;
        progressSlider.value = progress;
        progressText.text = $"{currentEffect.tokensContributed} / {currentEffect.tokensRequired} tokens contributed";

        // Show consequences
        string consequences = "If not prevented: \n";
        foreach (var effect in currentEffect.effectsToApply)
        {
            consequences += $"- {effect.effectName}\n";
        }
        consequencesText.text = consequences;

        // Update contribution controls
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();
        int playerTokens = currentPlayer.tokenManager.GetTokens();
        currentPlayerTokensText.text = $"Your tokens: {playerTokens}";

        // Update button interactivity
        bool canContribute = !currentEffect.isCountered && !currentEffect.hasTriggered && playerTokens > 0;
        contribute1Button.interactable = canContribute && playerTokens >= 1;
        contribute5Button.interactable = canContribute && playerTokens >= 5;
        contributeAllButton.interactable = canContribute && playerTokens > 0;
        contributeButton.interactable = canContribute;

        // Update player contributions display
        UpdatePlayerContributionsDisplay();
    }

    private void UpdatePlayerContributionsDisplay()
    {
        // Clear existing displays
        foreach (Transform child in playerContributionsContainer)
        {
            Destroy(child.gameObject);
        }

        // Show each player's contribution
        var players = GameServices.Instance.turnManager.GetAllPlayers();
        foreach (var player in players)
        {
            int contribution = 0;
            if (currentEffect.playerContributions.TryGetValue(player.playerName, out contribution))
            {
                // Player has contributed
            }

            GameObject contributionObj = Instantiate(playerContributionPrefab, playerContributionsContainer);
            PlayerContributionDisplay display = contributionObj.GetComponent<PlayerContributionDisplay>();

            if (display != null)
            {
                bool isCurrentPlayer = player == GameServices.Instance.turnManager.GetCurrentPlayer();
                display.Setup(player.playerName, contribution, isCurrentPlayer);
            }
        }
    }

    private void ContributeTokens(int amount)
    {
        if (isProcessingContribution || currentEffect == null) return;

        isProcessingContribution = true;

        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();

        // Try to contribute
        bool success = GameServices.Instance.delayedCounteractionManager.ContributeTokens(
            currentEffect.effectId, currentPlayer, amount);

        if (success)
        {
            UpdateDisplay();
        }

        Invoke("ResetContributionProvessing", 0.1f);
    }

    private void ContributeAllTokens()
    {
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();
        int allTokens = currentPlayer.tokenManager.GetTokens();

        if (allTokens > 0)
        {
            ContributeTokens(allTokens);
        }
    }

    private void ResetContributionProvessing()
    {
        isProcessingContribution = false;
    }

    private void OnDelayedEffectUpdated(DelayedCounterableEffect effect)
    {
        if (currentEffect != null && effect.effectId == currentEffect.effectId)
        {
            UpdateDisplay();
        }
    }

    private void OnDelayedEffectCountered(DelayedCounterableEffect effect)
    {
        if (currentEffect != null && effect.effectId == currentEffect.effectId)
        {
            UpdateDisplay();
            Debug.Log($"Effect {effect.description} was successfully countered!");
        }
    }

    private void OnTokensChanged(PlayerController player)
    {
        if (player == GameServices.Instance.turnManager.GetCurrentPlayer())
        {
            UpdateDisplay();
        }
    }
}
