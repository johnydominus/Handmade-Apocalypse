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

    public PlayerController CurrentPlayer => currentPlayer;

    public TextMeshProUGUI turnHeaderText;
    public TextMeshProUGUI turnNumberText;
    public TokenUI tokenUI;
    public RegionUI regionUI;
//    public DevTools devTools;

    public void Initialize(List<PlayerController> playerList)
    {
        players = playerList;
        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
    }

    public void StartTurn()
    {
        currentPlayer = players[currentPlayerIndex];

        currentPlayer.RefillTokens();
        regionUI.SetRegion(currentPlayer, currentPlayer);
        tokenUI.player = currentPlayer;
        tokenUI.UpdateDisplay();
        turnHeaderText.text = $"{currentPlayer.playerName}'s turn";
//        devTools.SetTargetPlayer(currentPlayer);
        turnNumberText.text = turnNumber.ToString();

        Debug.Log($"---\n{currentPlayer.playerName}'s turn!");

        GameEvents.OnTurnStart.Raise();
    }

    public void EndTurn()
    {
        GameEvents.OnTurnEnd.Raise();

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        
        if (currentPlayerIndex == 0)
        {
            turnNumber++;
            turnNumberText.text = turnNumber.ToString();
        }

        StartTurn();
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }

    /* OLD IMPLEMENTAION
    public void EndTurn()
    {
        GameEvents.OnTurnEnd.Raise();

        //Shifts to the next turn after Player 2 hits "End Turn"
        if (currentPlayer == player2)
        {
            turnNumber++;
            turnNumberText.text = turnNumber.ToString();
        }

        currentPlayer = currentPlayer == player1 ? player2 : player1;
        tokenUI.player = currentPlayer;
        tokenUI.UpdateDisplay();
        turnHeaderText.text = $"{currentPlayer.playerName}'s turn";
        Debug.Log($"---\n{currentPlayer.playerName}'s turn!");
    //    cardSpawner.DrawHand(currentPlayer);
        regionUI.SetRegion(currentPlayer, currentPlayer); // start of player’s own turn
        devTools.SetTargetPlayer(currentPlayer);
        investmentManager.TickInvestments();
        GameEvents.OnTurnStart.Raise();
    }
    */
}
