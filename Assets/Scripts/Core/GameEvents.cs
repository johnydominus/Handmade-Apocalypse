using UnityEngine;

public static class GameEvents
{
    public static GameEvent OnGameInitialized = new GameEvent();
    public static GameEvent OnTurnStart = new GameEvent();
    public static GameEvent OnTurnEnd = new GameEvent();
    public static GameEventWithParam<ThreatType> OnThreatChanged = new GameEventWithParam<ThreatType>();
    public static GameEvent OnVictory = new GameEvent();
    public static GameEvent OnLoss = new GameEvent();
    public static GameEventWithParam<CardData> OnCardDrawn = new GameEventWithParam<CardData>();
    public static GameEventWithParam<CardData> OnCardPlayed = new GameEventWithParam<CardData>();
}
