using UnityEngine;

public struct CardPlayContext
{
    public CardData card;
    public PlayerController player;

    public CardPlayContext(CardData card, PlayerController player)
    {
        this.card = card;
        this.player = player;
    }
}
