using UnityEngine;

public class CardsListenerTest: MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnCardDrawn.RegisterListener(OnCardDrawn);
        GameEvents.OnCardPlayed.RegisterListener(OnCardPlayed);
    }

    private void OnDisable()
    {
        GameEvents.OnCardDrawn.UnregisterListener(OnCardDrawn);
        GameEvents.OnCardPlayed.UnregisterListener(OnCardPlayed);
    }

    private void OnCardDrawn(CardData card)
    {
        Debug.Log($"🟢 Card Drawn: {card.cardName} [{card.cardType}]");
    }

    private void OnCardPlayed(CardData card)
    {
        Debug.Log($"🔴 Card Played: {card.cardName} → Threat {card.targetThreat} change: {card.threatModifier}");
    }
}

