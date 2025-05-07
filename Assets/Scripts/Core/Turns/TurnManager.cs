using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Unity;

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
        // GameServices.Instance.effectManager.TickTurn();             // TODO: change the method according to the separate effects process
        Debug.Log("Turn cycle started!");
        NextPhase();
    }

    public void EndTurn()
    {
        GameEvents.OnTurnEnd.Raise();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        
        if (currentPlayerIndex == 0)
        {
            turnNumber++;
        }

        StartTurn();
    }

    public void NextPhase()
    {
        if (isPhaseTransitioning) return;
        isPhaseTransitioning = true;

        switch (currentPhase)
        {
            case TurnPhase.GlobalThreats:
                HandleGlobalThreats();
                currentPhase = TurnPhase.GlobalEvents;
                break;
            case TurnPhase.GlobalEvents:
                HandleGlobalEvents();
                currentPhase = TurnPhase.StartPlayerTurn;
                break;
            case TurnPhase.StartPlayerTurn:
                // StartTurn();
                Debug.Log("Starting player turn...");
//                GameEvents.OnTurnStarted.Raise(new TurnContext(turnNumber, currentPlayer));
                Debug.Log($"---\n{currentPlayer.playerName}'s turn!");
                currentPhase = TurnPhase.RegionEvents;
                break;
            case TurnPhase.RegionEvents:
                HandleRegionEvents();
                currentPhase = TurnPhase.DividendPayout;
                break;
            case TurnPhase.DividendPayout:
                GameServices.Instance.investmentManager.TickInvestments(); // TODO: change the method according to the individual payout process
                currentPhase = TurnPhase.PlayerAction;
                break;
            case TurnPhase.PlayerAction:
                // Let the player play
                // Pass the phase on button click
                break;
            case TurnPhase.EndPlayerTurn:
                EndTurn();
                NextPhase();
                break;
        }
        isPhaseTransitioning = false;
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
                messageText += $"New threat emmerged - it's {threat.threatType.ToString()}!\n";
            }
        } else
        {
            // TODO: Check new global threat trigger
            // 
        }

        Debug.Log($"Message header: {messageHeader}");
        Debug.Log($"Message text: {messageText}");

        currentPhase = TurnPhase.GlobalEvents;
        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log($"Proceeding to the {currentPhase.ToString()} phase...");
            GameServices.Instance.turnManager.NextPhase();
        });

        // Gather global threats on turn 1  V
        // Check new global threat trigger  
        // IF new global event triggerred   
        // Apply global threats change      
        // Update UI                        
        // Show a message                   V
        // Pass the phase on button click   V
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
            Debug.Log($"Card name: {card.name}");
            Debug.Log($"Card description: {card.description}");
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

        currentPhase = TurnPhase.StartPlayerTurn;
        MessagePanel.Show(messageHeader, messageText, () =>
        {
            Debug.Log($"Proceeding to the {currentPhase.ToString()} phase...");
            GameServices.Instance.turnManager.NextPhase();
        });

        // Apply global events mechanic     V
        // IF global event is triggered     
        //      Update UI
        // Show a message                   V
        // Pass the phase on button click   V
    }

    private void HandleRegionEvents()
    {
        // Apply region events change
        // Update UI
        // Show a message
        // Pass the phase on button click
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
