using UnityEngine;

public static class GameEvents
{
    public static GameEvent OnGameInitialized = new GameEvent();
    public static GameEvent OnTurnEnd = new GameEvent();
    public static GameEvent OnVictory = new GameEvent();
    public static GameEvent OnLoss = new GameEvent();
    
    public static GameEventWithParam<ThreatType> OnThreatChanged = new GameEventWithParam<ThreatType>();
    public static GameEventWithParam<CardPlayContext> OnCardPlayedWithOwner = new GameEventWithParam<CardPlayContext>();
    public static GameEventWithParam<TurnContext> OnTurnStarted = new GameEventWithParam<TurnContext>();
    public static GameEventWithParam<SoEContext> OnSoEActivated = new GameEventWithParam<SoEContext>();
    public static GameEventWithParam<CardData> OnCardDrawn = new GameEventWithParam<CardData>();
    public static GameEventWithParam<PlayerController> OnHandDrawn = new GameEventWithParam<PlayerController>();
    public static GameEventWithParam<PlayerController> OnTokenSpent = new GameEventWithParam<PlayerController>();
    public static GameEventWithParam<PlayerController> OnTokensChanged = new GameEventWithParam<PlayerController>();
}
