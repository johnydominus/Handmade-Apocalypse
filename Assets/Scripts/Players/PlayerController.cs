using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TokenManager tokenManager { get; private set; }
    private List<CardData> hand = new();
    public List<InvestmentSlot> investments = new();
    public List<Emergency> emergencies = new();

    public string playerName;
    public int startingTokensAmount = 6;
    public int startingEmergencyLevel = 3;

    public List<CardData> GetHand() => hand;
    public List<InvestmentSlot> GetInvestmentSlots() => investments;
    public void Initialize(List<SphereType> sphereNames, List<PlayerController> players)
    {
        tokenManager = new TokenManager();
        tokenManager.Initialize(startingTokensAmount, this);
        Debug.Log($"{playerName} initialized with {startingTokensAmount} tokens.");

        investments.Clear();

        foreach (var name in sphereNames)
        {            
            var slot = new InvestmentSlot(name);
            var emergency = EmergencyMapping.GetBySphere(name).emergency;

            Debug.Log($"{playerName} initializing {emergency.ToString()} emergency");

            if (emergency != null) 
            {
                this.emergencies.Add(new Emergency(emergency, this, startingEmergencyLevel));
                Debug.Log($"{playerName} initialized {emergency.ToString()} emergency");
            }
            else
            {
                Debug.Log($"{playerName} has no emergency for {name} sphere");
            }

            Debug.Log($"{playerName} initializing {name} investment sphere");

            foreach (var otherPlayer in players)
                slot.investors[otherPlayer] = new InvestorData();

            investments.Add(slot);

            Debug.Log($"{playerName} initialized {slot.sphereName} investment sphere");
        }
        Debug.Log($"{investments.Count} investment spheres were initialized!");
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

    public void ClearHand()
    {
        hand.Clear();
    }
}
