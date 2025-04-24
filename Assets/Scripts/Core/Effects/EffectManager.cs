using NUnit.Framework;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable

public class EffectManager
{
    private List<ActiveEffect> activeEffects = new();

    public void RegisterEffect(Effect effect, PlayerController player)
    {
        activeEffects.Add(new ActiveEffect(effect, player));
    }

    public void ApplyEffect(Effect effect, PlayerController player)
    {
        GameServices.Instance.commandManager.ExecuteCommand(new ApplyEffectCommand(effect, player));
    }

    // This method is called at the start of each turn to update the status of all active effects
    public void TickTurn()
    {
        foreach (var effect in activeEffects.ToList())
        {
            effect.timing.turnsElapsed++;

            switch (effect.timing.effectTimingType)
            {
                case EffectTimingType.CurrentTurn:
                    effect.expired = true;
                    break;
                case EffectTimingType.MultiTurn:
                    if (effect.timing.turnsElapsed >= effect.timing.duration)
                        effect.expired = true;
                    break;
                case EffectTimingType.Delayed:
                    if (effect.timing.turnsElapsed >= effect.timing.delay)
                        effect.Activate();
                    break;
                case EffectTimingType.Recurring:
                    if(effect.timing.turnsElapsed % effect.timing.interval == 0)
                        effect.Activate();
                    if(effect.timing.turnsElapsed >= effect.timing.duration)
                        effect.expired = true;
                    break;
            }
        }
        activeEffects.RemoveAll(effect => effect.expired);
    }

    // Call this method for each target, sphere and player each turn
    public float ResolveValue(float baseValue, EffectTarget target, SphereType sphere, PlayerController? player = null)
    {
        var relevantEffects = activeEffects
            .Where(effect => effect.isActive
                && effect.effect.effectTarget == target
                && (effect.effect.sphereType == sphere || effect.effect.sphereType == SphereType.All))
            .OrderBy(effect => effect.effect.effectSource)
            .ToList();

        float result = baseValue;

        // Apply multiplicative first
        foreach (var effect in relevantEffects.Where(e => e.effect.effectType == EffectType.Multiply))
            result *= effect.effect.value;

        // Apply additive next
        foreach (var effect in relevantEffects.Where(e => e.effect.effectType == EffectType.Add))
            result += effect.effect.value;

        // Apply blocking last
        foreach (var effect in relevantEffects.Where(e => e.effect.effectType == EffectType.Block))
            result = 0;

        return Mathf.Floor(result);
    }

    public void ClearEffects()
    {
        activeEffects.Clear();
    }
}
