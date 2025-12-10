using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Unity;
using Mono.Cecil.Cil;

public enum TurnPhase
{
    GlobalThreats,
    GlobalEvents,
    StartPlayerTurn,
    RegionEvents,
    DividendPayout,
    PlayerAction,
    EndPlayerTurn
}

public class TurnManager
{
    private List<PlayerController> players;
    private TurnPhase currentPhase = TurnPhase.GlobalThreats;
    private PlayerController currentPlayer;
    private int currentPlayerIndex = 0;
    private int turnNumber = 1;
    private CardLibrary cardLibrary;
    private bool isPhaseTransitioning = false;

    public GameObject messagePanel;
    private Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();

    public void OnEnable()
    {
        GameEvents.OnGameInitialized.RegisterListener(OnGameInitialized);
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    public void OnDisable()
    {
        GameEvents.OnGameInitialized.UnregisterListener(OnGameInitialized);
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    public void OnGameInitialized()
    {

    }

    public void OnTurnStarted(TurnContext context)
    {

    }

    public void Initialize(List<PlayerController> playerList, GameObject messagePrefab, CardLibrary cardLibrary)
    {
        this.players = playerList;
        this.currentPlayerIndex = 0;
        this.currentPlayer = players[currentPlayerIndex];
        this.messagePanel = messagePrefab;
        this.cardLibrary = cardLibrary;
    }

    public void StartTurn()
    {
        currentPlayer = players[currentPlayerIndex];

        // Process turn-level effects at the start of a turn
        GameServices.Instance.effectManager.TickTurn();

        Debug.Log($"Turn {turnNumber} started for player {currentPlayer.playerName}");
        Debug.Log($"Starting with phase: {currentPhase}");

        NextPhase();
    }

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        if (currentPlayerIndex == 0)
        {
            GameEvents.OnTurnEnd.Raise();
            turnNumber++;
            currentPhase = TurnPhase.GlobalThreats;

            Debug.Log($"The last player played! Turn {turnNumber - 1} ended, starting Turn {turnNumber}");
        }
        else
        {
            currentPhase = TurnPhase.StartPlayerTurn;
            currentPlayer = players[currentPlayerIndex];
            Debug.Log($"Passing turn to player {currentPlayer.playerName}");
        }

        NextPhase();
    }

    public void NextPhase()
    {
        if (isPhaseTransitioning)
        {
            Debug.Log($"Phase transition already in progress for {currentPhase}, ignoring NextPhase call");
            return;
        }

        isPhaseTransitioning = true;
        Debug.Log($"===== STARTING PHASE: {currentPhase} =====");

        switch (currentPhase)
        {
            case TurnPhase.GlobalThreats:
                HandleGlobalThreats();
                break;
            case TurnPhase.GlobalEvents:
                HandleGlobalEvents();
                break;
            case TurnPhase.StartPlayerTurn:
                HandlePlayerTurnStart();
                break;
            case TurnPhase.RegionEvents:
                HandleRegionEvents();
                break;
            case TurnPhase.DividendPayout:
                HandleDividendPayout();
                break;
            case TurnPhase.PlayerAction:
                // Let the player play - this phase is handled by UI
                Debug.Log("Player action phase - waiting for player input");
                isPhaseTransitioning = false;
                // No need to call NextPhase here as it will be triggered by player action
                break;
            case TurnPhase.EndPlayerTurn:
                Debug.Log("Ending player turn");

                // Using an empty message for consistency in phase transition
                MessagePanel.Show("", "", () => {
                    Debug.Log("End Player Turn phase callback executed");
                    isPhaseTransitioning = false;
                    EndTurn();
                });
                break;
        }
    }

    private void HandleGlobalThreats()
    {
        string messageHeader = "";
        string messageText = "";

        Debug.Log("Handling global threats...");
        Debug.Log($"Turn number is {turnNumber}");

        // Process threat-related effects before handling threats
        GameServices.Instance.effectManager.ProcessPhaseEffects(EffectProcessingPhase.GlobalThreats);

        if (turnNumber == 1)
        {
            // First turn initialization of threats
            List<Threat> activeThreats = GameServices.Instance.threatManager.GetThreats();
            Debug.Log($"Active threats: {activeThreats.Count}");

            if (activeThreats.Count == 1)
                messageHeader = "NEW GLOBAL THREAT!";
            else
                messageHeader = "NEW GLOBAL THREATS!";

            foreach (Threat threat in activeThreats)
            {
                messageText += $"New threat emerged - it's {threat.threatType.ToString()}!\n";
            }
        }
        else
        {
            // Select one random active threat to increase
            List<Threat> activeThreats = GameServices.Instance.threatManager.GetThreats();

            // Filter out the Asteroid threat
            List<Threat> threatsForRandomGrowth = activeThreats
                .Where(threat => threat.threatType != ThreatType.Asteroid)
                .ToList();

            UpdateAsteroid();

            if (threatsForRandomGrowth.Count > 0)
            {
                // Select a random threat
                int randomIndex = UnityEngine.Random.Range(0, threatsForRandomGrowth.Count);
                Threat selectedThreat = threatsForRandomGrowth[randomIndex];

                // Get base growth value (2-12)
                int baseGrowth = UnityEngine.Random.Range(2, 13);

                // Convert threat type to corresponding sphere for effect processing
                SphereType sphereType = EmergencyMapping.GetByThreat(selectedThreat.threatType).sphere;

                // Apply effect modifiers to the base change
                float modifiedGrowth = GameServices.Instance.effectManager.ResolveThreatModifier(baseGrowth, sphereType);

                // Apply the modified threat change
                int intModifiedGrowth = Mathf.RoundToInt(modifiedGrowth);
                GameServices.Instance.threatManager.ApplyThreatChange(selectedThreat.threatType, intModifiedGrowth);

                messageHeader = "THREAT LEVEL UPDATED";
                string direction = intModifiedGrowth > 0 ? "increased" : "decreased";
                messageText = $" {selectedThreat.threatType} {direction} by {Mathf.Abs(intModifiedGrowth)}!";

                Debug.Log($"Random threat {selectedThreat.threatType} {direction} by {Mathf.Abs(intModifiedGrowth)}%");
            }
            else if (activeThreats.Count > 0 && activeThreats.All(t => t.threatType == ThreatType.Asteroid))
            {
                Debug.Log("Only Asteroid threat is active, skipping random threat growth");
            }
            else
            {
                messageHeader = "GLOBAL THREATS";
                messageText = "No active threats for random growth";
            }
        }

        TurnPhase nextPhase = TurnPhase.GlobalEvents;
        Debug.Log($"Global Threats phase complete, next phase will be {nextPhase}");

        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log("Global Threats phase callback executed");
            currentPhase = nextPhase;
            isPhaseTransitioning = false;
            NextPhase();
        });
    }

    private void HandleGlobalEvents()
    {
        string messageHeader = "";
        string messageText = "";

        Debug.Log("Handling global events...");

        // Process Global Events effects before handling events
        GameServices.Instance.effectManager.ProcessPhaseEffects(EffectProcessingPhase.GlobalEvents);

        System.Random random = new();
        int random1 = random.Next(1, 6);
        int random2 = random.Next(1, 6);

        if (random1 == random2)
        {
            Debug.Log("Global event triggered!");

            CardData card = cardLibrary.GetRandomCard(CardType.GlobalEvent);
            Debug.Log($"Global Event card name: {card.name}");
            Debug.Log($"Global Event card description: {card.description}");
            messageHeader = "GLOBAL EVENT!";
            messageText = $"Global event triggered - {card.cardName}:\n{card.description}";

            ICommand command = new PlayGlobalEventCardCommand(card);
            GameServices.Instance.commandManager.ExecuteCommand(command);
            // TODO: Update UI
        }
        else
        {
            Debug.Log("No global event");
            messageHeader = "NO GLOBAL EVENT";
            messageText = $"{random1}:{random2} No new global event this turn";
        }

        TurnPhase nextPhase = TurnPhase.StartPlayerTurn;
        Debug.Log($"Global Events phase complete, next phase will be {nextPhase}");

        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log("Global Events phase callback executed");
            currentPhase = nextPhase;
            isPhaseTransitioning = false;
            NextPhase();
        });
    }

    private void HandlePlayerTurnStart()
    {
        string messageHeader = "";
        string messageText = "";

        Debug.Log("Starting player turn...");

        // Make sure we're using the correct player
        currentPlayer = players[currentPlayerIndex];

        // Process player specific effects at turn start
        GameServices.Instance.effectManager.ProcessPhaseEffects(EffectProcessingPhase.PlayerTurnStart);

        // Tick SoE blocking for current player
        foreach (var emergency in currentPlayer.emergencies)
        {
            emergency.stateOfEmergency.TickTurn();
        }

        // Tick delayed counterable effects (only once per turn cycle, not per player)
        if (currentPlayerIndex == 0)
        {
            GameServices.Instance.delayedCounteractionManager.TickAllDelayedEffects();
        }

        GameEvents.OnTurnStarted.Raise(new TurnContext(turnNumber, currentPlayer));

        Debug.Log($"---\n{currentPlayer.playerName}'s turn!");

        messageHeader = $"{currentPlayer.playerName}'s turn!";
        messageText = $"Do your job, {currentPlayer.playerName}!";

        TurnPhase nextPhase = TurnPhase.RegionEvents;
        Debug.Log($"Player Turn Start phase complete, next phase will be {nextPhase}");

        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log("Player Turn Start phase callback executed");
            currentPhase = nextPhase;
            isPhaseTransitioning = false;
            NextPhase();
        });
    }

    private void HandleRegionEvents()
    {
        string messageHeader = "";
        string messageText = "";

        Debug.Log($"Handling region event for player {currentPlayer.playerName}...");

        // Process Region Events effects before handling events
        GameServices.Instance.effectManager.ProcessPhaseEffects(EffectProcessingPhase.RegionEvents);

        // Apply emergency changes based on effects if needed
        foreach (var emergency in currentPlayer.emergencies)
        {
            // Get the sphere type for the emergency
            var sphereType = EmergencyMapping.GetByEmergency(emergency.emergencyType).sphere;

            // Check if there's any emergency related effects that should be applied
            float emergencyChange = GameServices.Instance.effectManager.ResolveEmergencyModifier(0, sphereType, currentPlayer);

            if (emergencyChange != 0)
            {
                // Apply the emergency change to the player's sphere
                if (emergencyChange > 0)
                    emergency.Increase(Mathf.FloorToInt(emergencyChange));
                else
                    emergency.Decrease(Mathf.CeilToInt(Mathf.Abs(emergencyChange)));

                Debug.Log($"Applied effect-based emergency change of {emergencyChange} to {emergency.emergencyType}");
            }
        }

        CardData card = cardLibrary.GetRandomCard(CardType.RegionEvent);

        Debug.Log($"Region Event card name: {card.name}");
        Debug.Log($"Region Event card description: {card.description}");

        messageHeader = $"REGION EVENT FOR {currentPlayer.playerName}!";
        messageText = $"{card.cardName}:\n{card.description}";

        // TODO: Execute region event card effects

        TurnPhase nextPhase = TurnPhase.DividendPayout;
        Debug.Log($"Region Events phase complete, next phase will be {nextPhase}");

        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log("Region Events phase callback executed");
            currentPhase = nextPhase;
            isPhaseTransitioning = false;
            NextPhase();
        });
    }

    private void HandleDividendPayout()
    {
        Debug.Log($"Handling dividend payout for player {currentPlayer.playerName}...");

        // Process dividend-related effects before payout
        GameServices.Instance.effectManager.ProcessPhaseEffects(EffectProcessingPhase.DividendsPayout, currentPlayer);

        // Using effect resolution
        GameServices.Instance.investmentManager.TickInvestments(currentPlayer);

        TurnPhase nextPhase = TurnPhase.PlayerAction;
        Debug.Log($"Dividend Payout phase complete, next phase will be {nextPhase}");

        // Using an empty message to maintain the same flow pattern
        MessagePanel.Show("", "", () =>
        {
            Debug.Log("Dividend Payout phase callback executed");
            currentPhase = nextPhase;
            isPhaseTransitioning = false;
            NextPhase();
        });
    }

    private void UpdateAsteroid()
    {
        var asteroidThreat = GameServices.Instance.threatManager.GetThreats()
            .FirstOrDefault(t => t.threatType == ThreatType.Asteroid);

        if (asteroidThreat != null)
        {
            // Asteroid increases by 5 each turn
            Effect asteroidEffect = new Effect(
                EffectSource.AsteroidApproaching,
                EffectTarget.ThreatLevel,
                EffectType.Add,
                SphereType.Astronautics,
                5,
                null);

            GameServices.Instance.commandManager.ExecuteCommand(
                new ModifyThreatCommand(asteroidEffect));
        }

        // Check if players have enough investment to defeat Asteroid
        CheckAsteroidDefenseCondition();
    }

    private void CheckAsteroidDefenseCondition()
    {
        if (!GameServices.Instance.threatManager.GetThreats()
            .Any(t => t.threatType == ThreatType.Asteroid))
        {
            return; // No asteroid threat, no need to check
        }

        bool allPlayersInvested = true;
        int requiredTokens = 100;

        foreach (var player in GameServices.Instance.turnManager.GetAllPlayers())
        {
            var astronauticsSlot = player.investments
                .FirstOrDefault(s => s.sphereName == SphereType.Astronautics);

            if (astronauticsSlot == null)
            {
                allPlayersInvested = false;
                break;
            }

            int totalInvestment = 0;
            foreach (var investorData in astronauticsSlot.investors.Values)
                totalInvestment += investorData.investedTokens;

            if (totalInvestment < requiredTokens)
            {
                allPlayersInvested = false;
                break;
            }
        }

        if (allPlayersInvested) 
        {
            // Create and effect to deactivate the Asteroid threat
            Effect deactivationEffect = new Effect(
                EffectSource.AsteroidCounteracted,
                EffectTarget.DeactivateThreat,
                EffectType.Add,
                SphereType.Astronautics,
                0,
                null);

            GameServices.Instance.commandManager.ExecuteCommand(
                new DeactivateThreatCommand(deactivationEffect));
        }
    }

    public List<PlayerController> GetAllPlayers()
    {
        return players;
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
