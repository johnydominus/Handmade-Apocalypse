using System;
using System.Collections;
using System.Collections.Generic;
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
        // GameServices.Instance.effectManager.TickTurn();

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

        if (turnNumber == 1)
        {
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
            // TODO: Check new global threat trigger
        }

        TurnPhase nextPhase = TurnPhase.GlobalEvents;
        Debug.Log($"Global Threats phase complete, next phase will be {nextPhase}");

        // Ensure we have a message to display
        if (string.IsNullOrEmpty(messageHeader))
        {
            messageHeader = "GLOBAL THREATS";
            messageText = "No new global threats this turn.";
        }

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

        CardData card = cardLibrary.GetRandomCard(CardType.RegionEvent);

        Debug.Log($"Region Event card name: {card.name}");
        Debug.Log($"Region Event card description: {card.description}");

        messageHeader = $"REGION EVENT FOR {currentPlayer.playerName}!";
        messageText = $"{card.cardName}:\n{card.description}";

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

        GameServices.Instance.investmentManager.TickInvestments();

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

    public List<PlayerController> GetAllPlayers()
    {
        return players;
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
