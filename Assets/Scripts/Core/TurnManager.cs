using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;
    public TokenUI tokenUI;
    public RegionUI regionUI;
    public InvestmentManager investmentManager;
    public TextMeshProUGUI turnHeaderText;
    public TextMeshProUGUI turnNumberText;
    public TokenUI GetTokenUI() => tokenUI;
    public DevTools devTools;
    
    
    private int turnNumber = 1;
    private PlayerController currentPlayer;

    [SerializeField] private CardSpawner cardSpawner;
    public static TurnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentPlayer = player1;
        currentPlayer.RefillTokens();
        regionUI.SetRegion(currentPlayer, currentPlayer);
        tokenUI.player = currentPlayer;
        tokenUI.UpdateDisplay();
        turnHeaderText.text = $"{currentPlayer.playerName}'s turn";
        cardSpawner.DrawHand(currentPlayer);
        Debug.Log($"{currentPlayer.playerName}'s turn!");
        devTools.SetTargetPlayer(currentPlayer);
        turnNumberText.text = turnNumber.ToString();
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void EndTurn()
    {
        cardSpawner.ClearHand();
        
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
        cardSpawner.DrawHand(currentPlayer);
        regionUI.SetRegion(currentPlayer, currentPlayer); // start of player’s own turn
        devTools.SetTargetPlayer(currentPlayer);
        investmentManager.TickInvestments();
    }
}
