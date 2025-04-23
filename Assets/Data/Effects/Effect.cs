using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

[System.Serializable] public class Effect
{
    public EffectSource effectSource;
    public EffectTarget effectTarget;
    public EffectType effectType;
    public SphereType sphereType;
    public EffectTiming effectTiming;                       // Default value for effect timing
    public int value;
    [NonSerialized] public PlayerController? targetPlayer;  // Player that the effect is applied to
    
    public Effect(EffectSource effectSource, EffectTarget effectTarget, EffectType effectType, SphereType sphereType, int value, PlayerController targetPlayer)
    {
        this.effectSource = effectSource;
        this.effectTarget = effectTarget;
        this.effectType = effectType;
        this.sphereType = sphereType;
        this.value = value;
        this.targetPlayer = targetPlayer;
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
