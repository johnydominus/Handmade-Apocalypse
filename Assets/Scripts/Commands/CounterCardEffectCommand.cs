using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterCardEffectCommand : ICommand
{
    private readonly Effect counterEffect;
    private readonly PlayerController player;
    private readonly List<CounteredEffectData> counteredEffects = new();

    // Store data for Undo functionality
    private struct CounteredEffectData
    {
        public ActiveEffect targetEffect;
        public float originalValue;
        public bool wasActive;
    }

    public CounterCardEffectCommand(Effect counterEffect, PlayerController player)
    {
        this.counterEffect = counterEffect;
        this.player = player;
    }

    public void Execute()
    {
        Debug.Log($"Executing counter effect targeting: {string.Join(", ", counterEffect.targetCardNames)}");

        // Get all active effects based on scope
        List<ActiveEffect> candidateEffects = GetEffectsInScope();

        // Filter to target cards
        List<ActiveEffect> targetEffects = candidateEffects.
            Where(effect => counterEffect.targetCardNames.Contains(effect.effect.sourceCardName))
            .ToList();

        Debug.Log($"Found {targetEffects.Count} effects to counter");

        // Apply counter effect to each target
        foreach (var targetEffect in targetEffects)
        {
            ApplyCounterToEffect(targetEffect);
        }

        // Register the counter effect itself if it has duration
        if (counterEffect.effectTiming.effectTimingType != EffectTimingType.Immediate)
        {
            GameServices.Instance.effectManager.RegisterEffect(counterEffect, player);
        }
    }

    private List<ActiveEffect> GetEffectsInScope()
    {
        var allActiveEffects = GameServices.Instance.effectManager.GetActiveEffects();

        switch (counterEffect.counterScope)
        {
            case CounterEffectScope.CurrentRegion:
                // Filter to effects that belong to the current player's region
                return allActiveEffects
                    .Where(e => e.player == player ||
                               (e.effect.sourceCardType == CardType.RegionEvent && e.player == player))
                    .ToList();

            case CounterEffectScope.GlobalEvents:
                // Include effects from Global Events (typically player is null)
                return allActiveEffects
                    .Where(e => e.effect.sourceCardType == CardType.GlobalEvent)
                    .ToList();

            case CounterEffectScope.AllRegions:
                // Include all player-specific effects
                return allActiveEffects
                    .Where(e => e.player != null)
                    .ToList();

            default:
                return allActiveEffects.ToList();
        }
    }

    private void ApplyCounterToEffect(ActiveEffect targetEffect)
    {
        // Store original state for undo
        CounteredEffectData undoData = new CounteredEffectData
        {
            targetEffect = targetEffect,
            originalValue = targetEffect.effect.value,
            wasActive = targetEffect.isActive
        };
        counteredEffects.Add(undoData);

        switch (counterEffect.counterType)
        {
            case CounterEffectType.CompleteNegation:
                targetEffect.Deactivate();
                Debug.Log($"Completely negated effect {targetEffect.effect.effectName}");
                break;

            case CounterEffectType.ValueReduction:
                targetEffect.effect.value -= Mathf.FloorToInt(counterEffect.counterValue);
                Debug.Log($"Reduced effect {targetEffect.effect.effectName} value by {counterEffect.counterValue}. New value: {targetEffect.effect.value}");
                break;

            case CounterEffectType.ValueMultiplication:
                targetEffect.effect.value *= Mathf.FloorToInt(counterEffect.counterValue);
                Debug.Log($"Multiplied effect {targetEffect.effect.effectName} value by {counterEffect.counterValue}. New value: {targetEffect.effect.value}");
                break;
        }
    }

    public void Undo()
    {
        Debug.Log($"Undoing counter effect for {counteredEffects.Count} effects");

        foreach (var undoData in counteredEffects)
        {
            // Restore original values
            undoData.targetEffect.effect.value = Mathf.FloorToInt(undoData.originalValue);

            if (undoData.wasActive)
                undoData.targetEffect.Activate();
            else
                undoData.targetEffect.Deactivate();

            Debug.Log($"Restored effect {undoData.targetEffect.effect.effectName} to original state");

            counteredEffects.Clear();
        }
    }
}
