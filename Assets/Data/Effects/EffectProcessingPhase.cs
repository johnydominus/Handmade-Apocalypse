using UnityEngine;

public enum EffectProcessingPhase
{
    GlobalThreats,      // Process effects that affect global threats
    GlobalEvents,       // Process effects that affect global events
    PlayerTurnStart,    // Process effects that at the player's turn start
    RegionEvents,       // Process effects during region events phase
    DividendsPayout,    // Process effects that affect dividends
    PlayerAction,       // Process effects during player actions
    TurnEnd,            // Process effects at the end of the turn
    Any                 // Process effects during any phase
}
