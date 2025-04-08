using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThreatType { Pandemic, NuclearWar, Asteroid }

[System.Serializable]
public class CardData
{
    public string cardName;
    public string description;
    public ThreatType threatTarget; // Which threat it affects
    public int threatChangeAmount;  // e.g. -10
    public int cardCost;

    public CardData(string name, string desc, ThreatType target, int change, int cardCost)
    {
        this.cardName = name;
        this.description = desc;
        this.threatTarget = target;
        this.threatChangeAmount = change;
        this.cardCost = cardCost;
    }
}
