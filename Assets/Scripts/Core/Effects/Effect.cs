using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

[System.Serializable] public class Effect
{
    public EffectSource effectSource;   // Where it comes from - Global Event, Region Event, Player Action, SoE
    public EffectTarget effectTarget;   // What it affects - dividends, threat level, etc.
    public EffectType effectType;       // Add, multiply or block
    public SphereType sphereType;       // Sphere that the effect is related to (Medicine, Ecology, etc.)
    public EffectTiming effectTiming;   // Default value for effect timing
    public int value;                   // Value of the effect - how much it adds/
    [NonSerialized] PlayerController? player;

    public Effect(EffectSource effectSource, EffectTarget effectTarget, EffectType effectType, SphereType sphereType, int value, PlayerController? player)
    {
        this.effectSource = effectSource;
        this.effectTarget = effectTarget;
        this.effectType = effectType;
        this.sphereType = sphereType;
        this.value = value;
        this.player = player;
        this.effectTiming = new EffectTiming(); // Initialize with default values
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
    PlayerAction
}

public enum EffectTarget
{
    General,
    Dividends,
    ThreatLevel,
    EmergencyLevel,
    SoE
}

public enum EffectType
{
    Add,
    Multiply,
    Block
}
