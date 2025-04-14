using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TokenManager tokenManager { get; private set; }
    private List<CardData> hand = new List<CardData>();

    public string playerName;
    public int tokens = 6;

    public bool hasPlayedOnce = false;
    public int[] investmentLevels = new int[3]; // for 2 spheres
    public int[] investmentTimers = new int[3]; // turns held

    public float[] emergencyLevels = new float[2];
    public bool[] isEmergencyActive = new bool[2];

    public int[,] incomingInvestments = new int[3, 2];  // [sphereIndex, investorIndex]
    public int[] dividendCounter = new int[3];          // Tracks turns until 3-turn payout
    public int[,] slowDividendCounters = new int[3, 2]; // [sphere, sphereOwnerIndex]

    public void Initialize()
    {
        tokenManager = new TokenManager();
        tokenManager.Initialize(tokens);
        Debug.Log($"{playerName} initialized with {tokens} tokens.");
    }

    private void OnEnable()
    {
        GameEvents.OnTurnStart.RegisterListener(OnTurnStart);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStart.UnregisterListener(OnTurnStart);
    }

    private void OnTurnStart()
    {
        if (!IsMyTurn()) return;

        int drawCount = hasPlayedOnce ? 1 : 3;
        hasPlayedOnce = true;

        for (int i = 0; i < drawCount; i++)
            DrawCard(CardType.PlayerAction);

        FindFirstObjectByType<CardUIController>().DisplayHand(this);
    }


    public void DrawCard(CardType type)
    {
        CardData card = GameServices.Instance.cardSystem.DrawCard(type);
        if (card != null && type == CardType.PlayerAction)
        {
            hand.Add(card);
            Debug.Log($"{playerName} drew a card: {card.cardName}");
        }
    }

    public void DrawTurnCards()
    {
        int drawCount = hasPlayedOnce ? 1 : 3;
        hasPlayedOnce = true;

        for (int i = 0; i < drawCount; i++)
            DrawCard(CardType.PlayerAction);

        FindFirstObjectByType<CardUIController>().DisplayHand(this);
    }


    public bool SpendToken(int amount)
    {
        if (tokens >= amount)
        {
            tokens -= amount;
            Debug.Log($"{playerName} spent a token. Tokens left: {tokens}");
            return true;
        }
        Debug.Log($"{playerName} has no tokens left.");
        return false;
    }

    public void RefillTokens()
    {
        tokens = 6;
        Debug.Log($"{playerName}'s tokens refilled.");
    }

    public void PlayCard(CardData card)
    {
        if (!hand.Contains(card)) return;

        hand.Remove(card);
        GameServices.Instance.cardSystem.PlayCard(card, this);
    }

    public List<CardData> GetHand() => hand;

    public void ClearHand()
    {
        hand.Clear();
    }

    public bool IsMyTurn()
    {
        return GameServices.Instance.turnManager.CurrentPlayer == this;
    }

}
