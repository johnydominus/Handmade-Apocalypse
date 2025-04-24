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

    public bool PlayCard(CardData card, PlayerController player)
    {
        if (!player.tokenManager.HasEnoughTokens(card.tokenCost))
            return false;

        GameServices.Instance.commandManager.ExecuteCommand(new PlayActionCardCommand(card, player));
        GameEvents.OnCardPlayedWithOwner.Raise(new CardPlayContext(card, player));

        return true;
    }
}
