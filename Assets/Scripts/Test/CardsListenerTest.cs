using UnityEngine;

public class CardsListenerTest: MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnCardDrawn.RegisterListener(OnCardDrawn);
        GameEvents.OnCardPlayedWithOwner.RegisterListener(OnCardPlayedWithOwner);
    }

    private void OnDisable()
    {
        GameEvents.OnCardDrawn.UnregisterListener(OnCardDrawn);
        GameEvents.OnCardPlayedWithOwner.UnregisterListener(OnCardPlayedWithOwner);
    }

    private void OnCardDrawn(CardData card)
    {
        Debug.Log($"🟢 Card Drawn: {card.cardName} [{card.cardType}]");
    }

    private void OnCardPlayedWithOwner(CardPlayContext cardPlayContext)
    {
        Debug.Log($"🔴{cardPlayContext.player.playerName} played card : {cardPlayContext.card.cardName} → Threat {cardPlayContext.card.targetThreat} change: {cardPlayContext.card.threatModifier}");
    }
}

