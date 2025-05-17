using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor;
using System.ComponentModel.Design;

public class AsteroidCounteractionPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button investButton;

    [Header("Display Elements")]
    [SerializeField] private TextMeshProUGUI threatLevelText;
    [SerializeField] private TextMeshProUGUI totalInvestedText;
    [SerializeField] private Transform playerInvestmentsContainer;
    [SerializeField] private GameObject playerInvestmentPrefab;

    [Header("Configuration")]
    [SerializeField] private int investmentCost = 50;

    private Dictionary<PlayerController, int> playerInvestments = new();
    private List<GameObject> playerDisplays = new();

    private void Awake()
    {
        // Hide panel initially
        panelRoot.SetActive(false);

        // Set up button listeners
        closeButton.onClick.AddListener(ClosePanel);
        investButton.onClick.AddListener(InvestToken);
    }

    private void OnEnable()
    {
        // Subscribe to player investment events
        GameEvents.OnThreatChanged.RegisterListener(OnThreatChanged);
        GameEvents.OnTokenSpent.RegisterListener(OnTokenSpent);
    }

    private void OnDisable()
    {
        // Unsubscribe from player investment events
        GameEvents.OnThreatChanged.UnregisterListener(OnThreatChanged);
        GameEvents.OnTokenSpent.UnregisterListener(OnTokenSpent);
    }

    public void OpenPanel()
    {
        // Get current asteroid threat info
        var asteroid = GameServices.Instance.threatManager.GetThreats()
            .Find(t => t.threatType == ThreatType.Asteroid);

        if (asteroid == null)
        {
            Debug.LogWarning("Tried to open Asteroid panel but threat is not active");
            return;
        }

        // Update threat level display
        threatLevelText.text = $"Asteroid Threat Level: {asteroid.threatValue}%";

        // Initialize or update player investments
        InitializePlayerInvestments();

        // Calculate and display total invested tokens
        UpdateTotalInvestments();

        // Show the panel
        panelRoot.SetActive(true);
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
    }

    public void InvestToken()
    {
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();

        // Try to spend a token
        if (currentPlayer.SpendToken(1))
        {
            // If successful, record the investment
            if (!playerInvestments.ContainsKey(currentPlayer))
                playerInvestments[currentPlayer] = 0;

            playerInvestments[currentPlayer]++;

            // Update displays
            UpdatePlayerInvestments();
            UpdateTotalInvestments();

            // Check if we've reached the required total
            CheckCompletionStatus();
        }
    }

    private void InitializePlayerInvestments()
    {
        // Clear existing player investments
        foreach (var display in playerDisplays)
            Destroy(display);

        playerDisplays.Clear();

        // Create display for each player
        foreach (var player in GameServices.Instance.turnManager.GetAllPlayers())
        {
            // Ensure player is in dictionary
            if (!playerInvestments.ContainsKey(player))
                playerInvestments[player] = 0;

            // Create player investment display
            GameObject display = Instantiate(playerInvestmentPrefab, playerInvestmentsContainer);

            // Set player name and investment text
            TextMeshProUGUI nameText = display.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI investmentText = display.transform.Find("InvetmentAmount").GetComponent<TextMeshProUGUI>();

            nameText.text = player.playerName;
            investmentText.text = $"{playerInvestments[player]} / {investmentCost}";

            // Colorize base on current/target player
            Image background = display.GetComponent<Image>();
            bool isCurrentPlayer = player == GameServices.Instance.turnManager.GetCurrentPlayer();
            background.color = isCurrentPlayer ? new Color(0.8f, 0.9f, 1f) : Color.white;

            playerDisplays.Add(display);
        }
    }

    private void UpdatePlayerInvestments()
    {
        // Update each player's investment display
        for (int i = 0; i < playerDisplays.Count; i++)
        {
            var player = GameServices.Instance.turnManager.GetAllPlayers()[i];
            var display = playerDisplays[i];

            TextMeshProUGUI investmentText = display.transform.Find("InvetmentAmount").GetComponent<TextMeshProUGUI>();
            investmentText.text = $"{playerInvestments[player]} / {investmentCost}";

            // Update highlight for current player
            Image background = display.GetComponent<Image>();
            bool isCurrentPlayer = player == GameServices.Instance.turnManager.GetCurrentPlayer();
            background.color = isCurrentPlayer ? new Color(0.8f, 0.9f, 1f) : Color.white;
        }
    }

    private void UpdateTotalInvestments()
    {
        // Calculate total invested tokens
        int total = 0;
        foreach (var investment in playerInvestments.Values)
        {
            total += investment;
        }

        // Calculate required total
        int playerCount = GameServices.Instance.turnManager.GetAllPlayers().Count;
        int requiredTotal = playerCount * investmentCost;

        // Update display
        totalInvestedText.text = $"Total Invested: {total} / {requiredTotal}";

        // Update invest button interactivity
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();
        investButton.interactable = currentPlayer.tokenManager.GetTokens() > 0;
    }

    private void CheckCompletionStatus()
    {
        // Calculate total invested tokens
        int total = 0;
        foreach (var investment in playerInvestments.Values)
            total += investment;

        // Calculate required total
        int playerCount = GameServices.Instance.turnManager.GetAllPlayers().Count;
        int requiredTotal = playerCount * investmentCost;

        // Check if we've reached the goal
        if (total >= requiredTotal)
        {
            // Create the effect to deactivate asteroid
            Effect deactivationEffect = new Effect(
                EffectSource.AsteroidCounteracted,
                EffectTarget.DeactivateThreat,
                EffectType.Add,
                SphereType.Astronautics,
                0,
                null);

            // Execute the effect
            GameServices.Instance.commandManager.ExecuteCommand(
                new DeactivateThreatCommand(deactivationEffect));

            // Close the panel
            ClosePanel();
        }
    }

    private void OnThreatChanged(ThreatType type)
    {
        // Update if asteroid panel is open
        if (type == ThreatType.Asteroid && panelRoot.activeSelf)
        {
            var asteroid = GameServices.Instance.threatManager.GetThreats()
                .Find(t => t.threatType == ThreatType.Asteroid);

            if (asteroid != null)
                threatLevelText.text = $"Asteroid threat Level: {asteroid.threatValue}%";
        }
    }

    private void OnTokenSpent(PlayerController player)
    {
        // Update button interactibility if current player spent tokens
        if (player == GameServices.Instance.turnManager.GetCurrentPlayer())
            investButton.interactable = player.tokenManager.GetTokens() > 0;
    }
}
