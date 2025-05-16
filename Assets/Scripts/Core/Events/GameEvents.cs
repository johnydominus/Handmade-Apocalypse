using UnityEngine;

public static class GameEvents
{
    public static GameEvent OnGameInitialized = new();
    public static GameEvent OnTurnEnd = new();
    public static GameEvent OnVictory = new();
    public static GameEvent OnLoss = new();
    
    public static GameEventWithParam<ThreatType> OnThreatChanged = new();
    public static GameEventWithParam<CardPlayContext> OnCardPlayedWithOwner = new();
    public static GameEventWithParam<TurnContext> OnTurnStarted = new();
    public static GameEventWithParam<SoEContext> OnSoEActivated = new();
    public static GameEventWithParam<SoEContext> OnSoEDeactivated = new();
    public static GameEventWithParam<CardData> OnCardDrawn = new();
    public static GameEventWithParam<PlayerController> OnHandDrawn = new();
    public static GameEventWithParam<PlayerController> OnTokenSpent = new();
    public static GameEventWithParam<PlayerController> OnTokensChanged = new();
    public static GameEventWithParam<ThreatType> OnThreatActivated = new();
    public static GameEventWithParam<ThreatType> OnThreatDeactivated = new();
}
