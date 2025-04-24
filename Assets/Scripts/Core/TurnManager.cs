using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TurnManager
{
    private List<PlayerController> players;
    private int currentPlayerIndex = 0;
    private PlayerController currentPlayer;
    private int turnNumber = 1;

    public void OnEnable()
    {
        GameEvents.OnGameInitialized.RegisterListener(OnGameInitialized);
    }

    public void OnDisable()
    {
        GameEvents.OnGameInitialized.UnregisterListener(OnGameInitialized);
    }

    public void OnGameInitialized()
    {
        StartTurn();
    }

    public void Initialize(List<PlayerController> playerList)
    {
        players = playerList;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
    }

    public void StartTurn()
    {
        currentPlayer = players[currentPlayerIndex];
        GameEvents.OnTurnStarted.Raise(new TurnContext(turnNumber, currentPlayer));
        GameServices.Instance.effectManager.TickTurn();
        Debug.Log($"---\n{currentPlayer.playerName}'s turn!");
    }

    public void EndTurn()
    {
        GameEvents.OnTurnEnd.Raise();

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        
        if (currentPlayerIndex == 0)
        {
            turnNumber++;
            GameServices.Instance.investmentManager.TickInvestments();
        }

        StartTurn();
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
