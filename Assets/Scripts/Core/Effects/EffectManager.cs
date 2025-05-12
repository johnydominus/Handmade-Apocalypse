using NUnit.Framework;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable

public class EffectManager
{
    private List<ActiveEffect> activeEffects = new();

    public void RegisterEffect(Effect effect, PlayerController? player)
    {
        var activeEffect = new ActiveEffect(effect, player);
        activeEffects.Add(activeEffect);

        Debug.Log($"Registered effect: {effect.effectName} " +
            (player != null ? $"for {player.playerName}" : "globally") +
            $" (process in {effect.processingPhase} phase)");
    }

    public void ApplyEffect(Effect effect, PlayerController? player)
    {
        GameServices.Instance.commandManager.ExecuteCommand(new ApplyEffectCommand(effect, player));
    }

    // This method is called at the start of each turn to update the status of all active effects
    public void TickTurn()
    {
        Debug.Log($"Ticking {activeEffects.Count} effects for new turn");

        foreach (var effect in activeEffects.ToList())
        {
            effect.timing.turnsElapsed++;

            Debug.Log($"Effect {effect.effect.effectName}: Turn {effect.timing.turnsElapsed}" +
                     (effect.player != null ? $" for {effect.player.playerName}" : ""));

            switch (effect.timing.effectTimingType)
            {
                case EffectTimingType.Immediate:
                    // Immediate effects should expire after being processed once
                    effect.expired = true;
                    Debug.Log($"Immediate effect {effect.effect.effectName} marked as expired");
                    break;

                case EffectTimingType.CurrentTurn:
                    // Current turn effects should expire at the end of the turn
                    effect.expired = true;
                    break;

                case EffectTimingType.MultiTurn:
                    // Multi-turn effects should expire after the specified duration
                    if (effect.timing.turnsElapsed >= effect.timing.duration)
                    {
                        effect.expired = true;
                        Debug.Log($"Multi-turn effect {effect.effect.effectName} expired after {effect.timing.duration} turns");
                    }
                    break;

                case EffectTimingType.Delayed:
                    // Delayed effects should activate after the specified delay
                    if (effect.timing.turnsElapsed >= effect.timing.delay && !effect.isActive)
                    {
                        effect.Activate();
                        Debug.Log($"Delayed effect {effect.effect.effectName} activated after {effect.timing.delay} turns");

                        // If it's a one-time effect, expire it after activation
                        if (effect.timing.duration <= 1)
                        {
                            effect.expired = true;
                            Debug.Log($"One-time delayed effect {effect.effect.effectName} marked as expired");
                        }
                    }
                    break;

                case EffectTimingType.Recurring:
                    // Recurring effects expire after their total duration
                    if(effect.timing.turnsElapsed >= effect.timing.duration)
                    {
                        effect.expired = true;
                        Debug.Log($"Recurring effect {effect.effect.effectName} expired after {effect.timing.duration} turns");
                    }
                    break;
            }
        }

        // Remove expired effects
        int removedCount = activeEffects.RemoveAll(effect => effect.expired);
        if (removedCount > 0)
            Debug.Log($"Removed {removedCount} expired effects");
    }

    // Process effects for a specific phase
    public void ProcessPhaseEffects(EffectProcessingPhase phase, PlayerController? currentPlayer = null)
    {
        Debug.Log($"Processing effects for phase: {phase}" +
                 (currentPlayer != null ? $" for player {currentPlayer.playerName}" : ""));

        // Find all active effects that should be processed in the phase
        var relevantEffects = activeEffects.Where(e =>
            e.isActive &&
            (e.effect.processingPhase == phase || e.effect.processingPhase == EffectProcessingPhase.Any) &&
            (currentPlayer == null || e.player == null || e.player == currentPlayer))
            .ToList();

        Debug.Log($"Found {relevantEffects.Count} relevant effects for phase {phase}");

        // Process recurring effects that should trigger this turn
        foreach (var effect in relevantEffects)
        {
            // For recuring effects, check if they should trigger this turn
            if (effect.timing.effectTimingType == EffectTimingType.Recurring
                && effect.timing.turnsElapsed % effect.timing.duration == 0)
            {
                Debug.Log($"Activating recurring effect {effect.effect.effectName} in {phase} phase");
                // For recurring effects, we might need to apply their effect directly
                ApplyEffectDirectly(effect.effect, effect.player);
            }
        }
    }

    // Apply the effect directly (without going through the command manager)
    // This is used for recurring effects that need to apply their effect each interval
    private void ApplyEffectDirectly(Effect effect, PlayerController? player)
    {
        switch (effect.effectTarget)
        {
            case EffectTarget.ThreatLevel:
                // Get the threat type for this sphere
                var threatType = EmergencyMapping.GetBySphere(effect.sphereType).threat;
                GameServices.Instance.threatManager.ApplyThreatChange(threatType, effect.value);
                Debug.Log($"Applied threat change of {effect.value} to {threatType}");
                break;

            case EffectTarget.EmergencyLevel:
                if (player != null)
                {
                    // Find the corresponding emergency
                    var emergencyType = EmergencyMapping.GetBySphere(effect.sphereType).emergency;
                    if (emergencyType != null)
                    {
                        var emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                        if (emergency != null)
                        {
                            if (effect.value > 0)
                                emergency.Increase(effect.value);
                            else
                                emergency.Decrease(effect.value);

                            Debug.Log($"Applied emergency change of {effect.value} to {emergencyType} for {player.playerName}");
                        }
                    }
                }
                break;

            // Other effect types will be handled with their values and retrieved wia ResolveValue
            default:
                Debug.Log($"Effect {effect.effectName} will be applied when its value is requested");
                break;
        }
    }

    // Callculate modified values based on active effects
    public float ResolveValue(float baseValue, EffectTarget target, SphereType sphere, EffectProcessingPhase currentPhase, PlayerController? player = null)
    {
        // Find all relevant effects that match the target, sphere, phase and player criteria
        var relevantEffects = activeEffects
            .Where(effect => effect.isActive
                && effect.effect.effectTarget == target
                && (effect.effect.sphereType == sphere || effect.effect.sphereType == SphereType.All)
                && (effect.effect.processingPhase == currentPhase || effect.effect.processingPhase == EffectProcessingPhase.Any)
                && (player == null || effect.player != null || effect.player == player))
            .OrderBy(effect => (int)effect.effect.effectSource)     // Sort by effect source hierarchy
            .ToList();

        if (relevantEffects.Count > 0)
            Debug.Log($"Found {relevantEffects.Count} relevant effects for {target} in {sphere} during {currentPhase}");
        else
            return baseValue;   // No relevant effects found, return base value unchanged

        float result = baseValue;

        // Process multipliers that affect other effects
        var multipliers = relevantEffects
            .Where(e => e.effect.effectType == EffectType.SphereEffectMultiplier)
            .ToList();

        // Helper function 
        float ApplyMultipliers(float val, bool isPositive)
        {
            foreach (var multiplier in multipliers)
            {
                if ((isPositive && multiplier.effect.appliesToPositive) ||
                    (!isPositive && multiplier.effect.appliesToNegative))
                {
                    float oldVal = val;
                    val *= multiplier.effect.multiplierValue;
                    Debug.Log($"Applied multiplier: {oldVal} * {multiplier.effect.multiplierValue} = {val}");
                }
            }
            return val;
        }

        // Apply multiplicative effects first (higher priority in calculation)
        foreach (var effect in relevantEffects.Where(e => e.effect.effectType == EffectType.Multiply))
        {
            float valToApply = effect.effect.value;
            valToApply = ApplyMultipliers(valToApply, valToApply > 1);  // Apply meta-multipliers
            
            float oldResult = result;
            result *= valToApply;

            Debug.Log($"Applied multiplicative effect {effect.effect.effectName}: {oldResult} * {valToApply} = {result}");
        }

        // Apply additive effects next
        foreach (var effect in relevantEffects.Where(e => e.effect.effectType == EffectType.Add))
        {
            float valToApply = effect.effect.value;
            valToApply = ApplyMultipliers(valToApply, valToApply > 0);  // Apply meta-multipliers

            float oldResult = result;
            result += valToApply;

            Debug.Log($"Applied additive effect {effect.effect.effectName}: {oldResult} + {valToApply} = {result}");
        }

        // Apply blocking effects last
        if (relevantEffects.Any(e => e.effect.effectType == EffectType.Block))
        {
            var blockingEffect = relevantEffects.First(e => e.effect.effectType == EffectType.Block);
            Debug.Log($"Applied blocking effect {blockingEffect.effect.effectName}: result set to 0 from {result}");
            result = 0;
        }

        // Round down to nearest integer as per game rules
        return Mathf.Floor(result);
    }

    // Convenience methods for specific phase-based value resolution
    public float ResolveThreatModifier(float baseValue, SphereType sphere)
        => ResolveValue(baseValue, EffectTarget.ThreatLevel, sphere, EffectProcessingPhase.GlobalThreats);

    public float ResolveDividendModifier(float baseValue, SphereType sphere, PlayerController player)
        => ResolveValue(baseValue, EffectTarget.Dividends, sphere, EffectProcessingPhase.DividendsPayout, player);

    public float ResolveEmergencyModifier(float baseValue, SphereType sphere, PlayerController player)
        => ResolveValue(baseValue, EffectTarget.EmergencyLevel, sphere, EffectProcessingPhase.RegionEvents, player);

    public float ResolveSoEModifier(float baseValue, SphereType sphere)
        => ResolveValue(baseValue, EffectTarget.SoE, sphere, EffectProcessingPhase.TurnEnd);

    public void ClearEffects()
    {
        Debug.Log($"Clearing all {activeEffects.Count} effects");
        activeEffects.Clear();
    }

    // Helper methods for debugging and UI
    public List<ActiveEffect> GetActiveEffects() => activeEffects.Where(e => e.isActive).ToList();

    public List<ActiveEffect> GetActiveEffectsForPlayer(PlayerController player)
        => activeEffects.Where(e => e.isActive && e.player == player).ToList();

    public List<ActiveEffect> GetActiveEffectsByTarget(EffectTarget target)
        => activeEffects.Where(e => e.isActive && e.effect.effectTarget == target).ToList();

    public List<ActiveEffect> GetActiveEffectsForPhase(EffectProcessingPhase phase)
        => activeEffects.Where(e => e.isActive &&
            (e.effect.processingPhase == phase || e.effect.processingPhase == EffectProcessingPhase.Any)).ToList();
}
