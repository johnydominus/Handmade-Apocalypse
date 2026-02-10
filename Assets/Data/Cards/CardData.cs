using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType 
{ 
    PlayerAction, 
    GlobalEvent,
    RegionEvent
}

public enum CardPolarity
{
    Positive,
    Negative,
    Neutral
}

[CreateAssetMenu(fileName = "Card", menuName = "Game/Card Data")]

public class CardData : ScriptableObject
{
    public CardType cardType;               // Type of card (Player Action, Global Event, Region Event)
    public SphereType sphereType;           // Sphere that the card is related to (Medicine, Ecology, etc.)
    public CardPolarity cardPolarity;        // Polarity of the card (Positive, Negative, Neutral)
    public string cardName;         
    public string description;      
    public int tokenCost;
    public Sprite artWork;

    public List<Effect> effects = new();    // Effects that are applied when the card is played

}
