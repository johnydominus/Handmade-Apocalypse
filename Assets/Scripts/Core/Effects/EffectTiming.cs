using UnityEngine;

[System.Serializable] public class EffectTiming
{
    public EffectTimingType effectTimingType;
    public int duration;        // Duration in turns for multi-turn or delayed effects
    public int delay;           // Delay in turns for delayed effects
    public int interval;        // For recurring effects
    public int turnsElapsed;    // For recurring and delayed effects

    public void Initialize()
    {
        // Initialization logic if needed
    }
}

public enum EffectTimingType
{
    Immediate,      // Takes effect at play
    CurrentTurn,    // Takes effect for the current turn
    MultiTurn,      // Takes effect for the several turns from the current
    Delayed,        // Takes effect after a delay of certain amount of turns
    Recurring       // Takes effect every certain amount of turns
}
