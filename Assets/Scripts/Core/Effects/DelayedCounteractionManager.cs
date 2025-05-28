using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelayedCounteractionManager
{
    private List<DelayedCounterableEffect> activeDelayedEffects = new();
    private int nextEffectId = 1;

    public void RegisterDelayedEffect(Effect effect, PlayerController sourcePlayer = null)
    {
        if (!effect.isCounterable)
        {
            Debug.LogWarning($"Attemped to register non-counterable effect as delayed counterable");
            return;
        }

        string effectId = $"delayed_{nextEffectId++}";
        string description = !string.IsNullOrEmpty(effect.counteractionDescription)
            ? effect.counteractionDescription
            : effect.effectName;

        var delayedEffect = new DelayedCounterableEffect(
            effectId,
            description,
            effect.effectTiming.delay,
            effect.tokensRequiredToCounter,
            effect.effectsIsNotCountered
        );

        activeDelayedEffects.Add(delayedEffect);

        Debug.Log($"Registered delayed counterable effect: '{description}' " +
            $"(Delay: {effect.effectTiming.delay} turns, Tokens needed: {effect.tokensRequiredToCounter})");

        GameEvents.OnDelayedEffectRegistered.Raise(delayedEffect);
    }

    public bool ContributeTokens(string effectId, PlayerController player, int amount)
    {
        var delayedEffect = activeDelayedEffects.FirstOrDefault(e => e.effectId == effectId);
        if (delayedEffect == null)
        {
            Debug.LogWarning($"Delayed effect {effectId} not found");
            return false;
        }

        return delayedEffect.AddTokens(player, amount);
    }

    public void TickAllDelayedEffects()
    {
        Debug.Log($"Ticking {activeDelayedEffects.Count} delayed effects");

        foreach (var delayedEffect in activeDelayedEffects.ToList())
        {
            delayedEffect.TicKTurn();
        }

        // Remove triggered or countered effects
        int removedCount = activeDelayedEffects.RemoveAll(e => e.hasTriggered);
        if (removedCount > 0)
        {
            Debug.Log($"Removed {removedCount} completed delayed effects");
        }
    }

    public List<DelayedCounterableEffect> GetActiveDelayedEffects()
    {
               return activeDelayedEffects.Where(e => !e.hasTriggered).ToList();
    }

    public DelayedCounterableEffect GetDelayedEffectById(string effectId)
    {
        return activeDelayedEffects.FirstOrDefault(e => e.effectId == effectId);
    }

    public void ClearAllDelayedEffects()
    {
        Debug.Log($"Clearing all {activeDelayedEffects.Count} delayed effects");
        activeDelayedEffects.Clear();
    }
}
