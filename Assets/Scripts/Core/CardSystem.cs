using System.Collections.Generic;
using UnityEngine;

// This class is responsible for managing the card system, including drawing cards and managing the player's hand
public class CardSystem
{
    private CardLibrary cardLibrary;

    public void Initialize(CardLibrary library)
    {
        this.cardLibrary = library;
    }

    public CardData DrawCard(CardType type)
    {
        CardData card = cardLibrary.GetRandomCard(type);
        GameEvents.OnCardDrawn.Raise(card);
        return card;
    }

    public void PlayCard(CardData card, PlayerController player)
    {
        if (!player.tokenManager.HasEnoughTokens(card.tokenCost))
        { 
            Debug.Log($"Not enough tokens to play {card.cardName}");
            return;
        }

        var command = new ModifyThreatCommand(card, player);
        GameServices.Instance.commandManager.ExecuteCommand(command);

        GameEvents.OnCardPlayed.Raise(card);
    }
}
