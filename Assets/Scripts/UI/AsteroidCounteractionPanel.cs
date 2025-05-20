using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AsteroidCounteractionPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button investButton;

    [Header("Display Elements")]
    [SerializeField] private TextMeshProUGUI threatLevelText;
    [SerializeField] private TextMeshProUGUI totalInvestedText;

    [Header("Player Displays")]
    [SerializeField] private GameObject player1Display;
    [SerializeField] private TextMeshProUGUI player1NameText;
    [SerializeField] private TextMeshProUGUI player1InvestmentText;
    [SerializeField] private Image player1Background;

    [SerializeField] private GameObject player2Display;
    [SerializeField] private TextMeshProUGUI player2NameText;
    [SerializeField] private TextMeshProUGUI player2InvestmentText;
    [SerializeField] private Image player2Background;

    private bool isProcessingInvestment = false;

    private void Awake()
    {
        // Hide panel initially
        panelRoot.SetActive(false);

        closeButton.onClick.RemoveAllListeners();
        investButton.onClick.RemoveAllListeners();

        // Set up button listeners
        closeButton.onClick.AddListener(ClosePanel);
        investButton.onClick.AddListener(InvestToken);

        Debug.Log("AsteroidCounteractionPanel initialized.");
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

        // Show the panel first to it's fully instantiated
        panelRoot.SetActive(true);

        // Update threat level display
        threatLevelText.text = $"Asteroid Threat Level: {asteroid.threatValue}%";

        // Initialize or update player investments
        InitializePlayerDisplays();

        // Calculate and display total invested tokens
        UpdateTotalInvestments();

        // Fix any layout issues
        FixLayoutPositioning();
    }

    private void FixLayoutPositioning()
    {
        // Disable any layout components that might be interfering
        if (player1Display != null)
        {
            DisableLayoutComponents(player1Display);
        }

        if (player2Display != null)
        {
            DisableLayoutComponents(player2Display);
        }

        // Make sure invest button is in front
        investButton.transform.SetAsLastSibling();
        closeButton.transform.SetAsLastSibling();
    }

    private void DisableLayoutComponents(GameObject obj)
    {
        // Disable layout groups
        LayoutGroup[] layoutGroups = obj.GetComponents<LayoutGroup>();
        foreach (var layout in layoutGroups)
        {
            layout.enabled = false;
        }

        // Disable content size fitters
        ContentSizeFitter[] fitters = obj.GetComponents<ContentSizeFitter>();
        foreach (var fitter in fitters)
        {
            fitter.enabled = false;
        }
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
    }

    public void InvestToken()
    {
        if (isProcessingInvestment) return;
        isProcessingInvestment = true;

        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();

        // Try to spend a token
        if (currentPlayer.SpendToken(1))
        {
            // If successful, add the investment using the manager
            AsteroidInvestmentManager.Instance.AddInvestment(currentPlayer, 1);

            // Update displays
            UpdatePlayerDisplays();
            UpdateTotalInvestments();

            // Check if we've reached the required total
            CheckCompletionStatus();
        }
        Invoke("ResetInvestmentProcessing", 0.1f);
    }

    private void ResetInvestmentProcessing()
    {
        isProcessingInvestment = false;
    }

    private void InitializePlayerDisplays()
    {
        var players = GameServices.Instance.turnManager.GetAllPlayers();

        // Handle player 1
        if (players.Count > 0)
        {
            player1Display.SetActive(true);
            player1NameText.text = players[0].playerName;

            int player1Investments = AsteroidInvestmentManager.Instance.GetPlayerInvestment(players[0]);
            player1InvestmentText.text = $"{player1Investments} / {AsteroidInvestmentManager.Instance.investmentCost}";
            
            player1Background.color = (players[0] == GameServices.Instance.turnManager.GetCurrentPlayer())
                ? new Color(0.8f, 0.9f, 1f) : Color.white;
        }
        else
        {
            player1Display.SetActive(false);
        }

        // Handle player 2
        if (players.Count > 1)
        {
            player2Display.SetActive(true);
            player2NameText.text = players[1].playerName;

            int player2Investments = AsteroidInvestmentManager.Instance.GetPlayerInvestment(players[1]);
            player2InvestmentText.text = $"{player2Investments} / {AsteroidInvestmentManager.Instance.investmentCost}";
            
            player2Background.color = (players[1] == GameServices.Instance.turnManager.GetCurrentPlayer())
                ? new Color(0.8f, 0.9f, 1f) : Color.white;
        }
        else
        {
            player2Display.SetActive(false);
        }
    }

    private void UpdatePlayerDisplays()
    {
        var players = GameServices.Instance.turnManager.GetAllPlayers();
        Debug.Log($"Updating displays for {players.Count} players");

        // Update player 1
        if (players.Count > 0)
        {
            int player1Investment = AsteroidInvestmentManager.Instance.GetPlayerInvestment(players[0]);
            player1InvestmentText.text = $"{player1Investment} / {AsteroidInvestmentManager.Instance.investmentCost}";
 
            player1Background.color = (players[0] == GameServices.Instance.turnManager.GetCurrentPlayer())
                ? new Color(0.8f, 0.9f, 1f) : Color.white;

            Debug.Log($"Updated Player1 ({players[0].playerName}) investment display: {player1Investment}");
        }

        // Update player 2
        if (players.Count > 1)
        {
            int player2Investment = AsteroidInvestmentManager.Instance.GetPlayerInvestment(players[1]);
            player2InvestmentText.text = $"{player2Investment} / {AsteroidInvestmentManager.Instance.investmentCost}";
            
            player2Background.color = (players[1] == GameServices.Instance.turnManager.GetCurrentPlayer())
                ? new Color(0.8f, 0.9f, 1f) : Color.white;
            
            Debug.Log($"Updated Player2 ({players[1].playerName}) investment display: {player2Investment}");
        }
    }
    private void UpdateTotalInvestments()
    {
        // Calculate total invested tokens
        int total = AsteroidInvestmentManager.Instance.GetTotalInvestment();

        // Calculate required total
        int playerCount = GameServices.Instance.turnManager.GetAllPlayers().Count;
        int requiredTotal = playerCount * AsteroidInvestmentManager.Instance.investmentCost;

        // Update display
        totalInvestedText.text = $"Total Invested: {total} / {requiredTotal}";

        // Update invest button interactivity
        PlayerController currentPlayer = GameServices.Instance.turnManager.GetCurrentPlayer();
        investButton.interactable = currentPlayer.tokenManager.GetTokens() > 0;
    }

    private void CheckCompletionStatus()
    {
        // Check if we've reached the goal
        if (AsteroidInvestmentManager.Instance.IsDefenseComplete())
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

            // Clear investments
            AsteroidInvestmentManager.Instance.ClearInvestments();

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
