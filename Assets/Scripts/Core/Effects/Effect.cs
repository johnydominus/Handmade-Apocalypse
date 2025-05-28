using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

[System.Serializable] public class Effect
{
    public string effectName = "";          // Name of the effect
    public EffectSource effectSource;       // Where it comes from - Global Event, Region Event, Player Action, SoE
    public EffectTarget effectTarget;       // What it affects - dividends, threat level, etc.
    public EffectType effectType;           // Add, multiply or block
    public SphereType sphereType;           // Sphere that the effect is related to (Medicine, Ecology, etc.)
    public EffectTiming effectTiming;       // Default value for effect timing
    public int value;                       // Value of the effect - how much it adds/
    [NonSerialized] PlayerController? player;

    public List<string> targetCardNames = new();   // List of cards to counter
    public CounterEffectScope counterScope = CounterEffectScope.CurrentRegion;
    public CounterEffectType counterType = CounterEffectType.CompleteNegation;
    public float counterValue = 0f;

    // Source tracking for counter-targeting
    public string sourceCardName = "";      // Name of the card that treated this effect
    public CardType sourceCardType;         // Type of the source card

    // Property for phase-specific processing
    public EffectProcessingPhase processingPhase = EffectProcessingPhase.Any;

    public bool appliesToPositive;
    public bool appliesToNegative;
    public float multiplierValue = 1.0f;    // like 2.0f (for x2), 0.5f (for halving)
    
    public bool hasCondition = false;
    public EffectCondition? condition;
    [SerializeReference]
    public List<Effect> alternativeEffects = new(); // Effects to apply if condition fails

    public bool isCounterable = false;
    public int tokensRequiredToCounter = 0;
    public string counteractionDescription = "";
    [SerializeReference]
    public List<Effect> effectsIsNotCountered = new();

    public Effect(EffectSource effectSource, EffectTarget effectTarget, EffectType effectType, SphereType sphereType, int value, PlayerController? player = null)
    {
        this.effectSource = effectSource;
        this.effectTarget = effectTarget;
        this.effectType = effectType;
        this.sphereType = sphereType;
        this.value = value;
        this.player = player;
        this.effectTiming = new EffectTiming(); // Initialize with default values
        this.effectName = GenerateEffectName();

        // Auto-assign processing phase based on effect target
        AssignProcessingPhase();
    }

    // Deep copy constructor
    public Effect Clone()
    {
        Effect clone = new Effect(
            effectSource,
            effectTarget,
            effectType,
            sphereType,
            value,
            player
        )
        {
            effectName = this.effectName,
            appliesToPositive = this.appliesToPositive,
            appliesToNegative = this.appliesToNegative,
            multiplierValue = this.multiplierValue,
            processingPhase = this.processingPhase
        };

        // Clone the timing data
        clone.effectTiming = new EffectTiming
        {
            effectTimingType = this.effectTiming.effectTimingType,
            duration = this.effectTiming.duration,
            delay = this.effectTiming.delay,
            interval = this.effectTiming.interval,
            turnsElapsed = this.effectTiming.turnsElapsed
        };

        return clone;
    }

    private string GenerateEffectName()
    {
        string action = effectType switch
        {
            EffectType.Add => value >= 0 ? "Increase" : "Decrease",
            EffectType.Multiply => value >= 1.0f ? "Multiply" : "Divide",
            EffectType.Block => "Block",
            EffectType.SphereEffectMultiplier => "Amlify",
            _ => "Modify"
        };

        string target = effectTarget.ToString();
        string sphere = sphereType == SphereType.All ? "all spheres" : sphereType.ToString();

        return $"{action} {target} in {sphere}";
    }

    // Helper method to automatically determine appropriate processing phase
    private void AssignProcessingPhase()
    {
        processingPhase = effectTarget switch
        {
            EffectTarget.ThreatLevel => EffectProcessingPhase.GlobalThreats,
            EffectTarget.Dividends => EffectProcessingPhase.DividendsPayout,
            EffectTarget.EmergencyLevel => EffectProcessingPhase.RegionEvents,
            EffectTarget.SoE => EffectProcessingPhase.TurnEnd,
            _ => EffectProcessingPhase.Any
        };
    }

    public void Inititalize()
    {

    }
}

public enum EffectSource
{
    GlobalEvent,
    SoE,
    RegionEvent,
    PlayerAction,
    GameStart,
    AsteroidApproaching,
    AsteroidCounteracted
}

public enum EffectTarget
{
    General,
    Dividends,
    ThreatLevel,
    EmergencyLevel,
    SoE,
    ActivateThreat,
    DeactivateThreat,
    PlayerTokens,
    CounterCardEffect,
    SoECounterAction,
    SoEBlock,
    DelayedCounterable
}

public enum CounterEffectScope
{
    CurrentRegion,
    AllRegions,
    GlobalEvents
}

public enum CounterEffectType
{
    CompleteNegation,
    ValueReduction,
    ValueMultiplication
}

public enum EffectType
{
    Add,
    Multiply,
    Block,
    SphereEffectMultiplier,
    Counteract
}
