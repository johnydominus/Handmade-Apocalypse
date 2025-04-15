using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TokenManager tokenManager { get; private set; }
    private List<CardData> hand = new List<CardData>();

    public string playerName;
    public int startingTokensAmount = 6;

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
        tokenManager.Initialize(startingTokensAmount, this);
        Debug.Log($"{playerName} initialized with {startingTokensAmount} tokens.");
    }

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        if (turnContext.player != this) return;

        if (turnContext.turnNumber == 1)
            RefillTokens();
        
        DrawTurnCards(turnContext.turnNumber);
    }

    public void DrawTurnCards(int turnNumber)
    {
        int drawCount = (turnNumber == 1) ? 3 : 1;

        for (int i = 0; i < drawCount; i++)
            DrawCard(CardType.PlayerAction);

        GameEvents.OnHandDrawn.Raise(this);
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

    public bool SpendToken(int amount)
    {
        if (this.tokenManager.GetTokens() >= amount)
        {
            tokenManager.SetTokens(tokenManager.GetTokens() - amount);
            GameEvents.OnTokenSpent.Raise(this);

            Debug.Log($"{playerName} spent a token. Tokens left: {tokenManager.GetTokens()}");
            return true;
        }
        Debug.Log($"{playerName} has no tokens left.");
        return false;
    }

    public void RefillTokens()
    {
        tokenManager.SetTokens(startingTokensAmount);
        Debug.Log($"{playerName}'s tokens refilled.");
    }

    public void PlayCard(CardData card)
    {
        if (!hand.Contains(card) || !tokenManager.HasEnoughTokens(card.tokenCost)) return;

        hand.Remove(card);
        GameEvents.OnCardPlayedWithOwner.Raise(new CardPlayContext(card, this));
        GameServices.Instance.cardSystem.PlayCard(card, this);
    }

    public List<CardData> GetHand() => hand;

    public void ClearHand()
    {
        hand.Clear();
    }
}
