using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StateOfEmergency
{
    List<Effect> effects = new();
    public bool isActive { get; private set; }
    public int tokensPut { get; private set; }
    public bool counteractionBlocked { get; private set; } = false; // Block counteraction
    public int blockedTurnsRemaining { get; private set; } = 0;     // Turns remaining for block

    private Emergency parent;
    private int tokensToDeactive = 12;

    public StateOfEmergency(Emergency parent)
    {
        this.parent = parent;
        isActive = false;
        tokensPut = 0;
        effects.Clear();
        effects.Add(new Effect(
                EffectSource.SoE,
                EffectTarget.Dividends,
                EffectType.Multiply,
                EmergencyMapping.GetByEmergency(parent.emergencyType).sphere, 0, parent.player));
    }

    public void Activate()
    {
        isActive = true;
        GameEvents.OnSoEActivated.Raise(new SoEContext(parent.emergencyType, parent.player));
        Debug.Log($"⚠️ Emergency #{parent.emergencyType} activated for {(parent.player.playerName)}!");
    }

    public void PutTokens(int amount)
    {
        if (counteractionBlocked)
        {
            Debug.LogWarning($"Counteraction blocked for {parent.emergencyType}. Cannot put tokens.");
            return;
        }

        tokensPut += amount;
        if (tokensPut >= tokensToDeactive)
            Deactivate();
    }

    // Removing tokens (for Undo functionality)
    public void RemoveTokens(int amount)
    {
        tokensPut -= Mathf.Max(0, tokensPut - amount);
        Debug.Log($"Removed {amount} tokens from {parent.emergencyType} SoE for {parent.player.playerName}. Remaining: {tokensPut}/{tokensToDeactive}");
    }

    // Block counteraction for a number of turns
    public void BlockCounteractionForTurns(int turns)
    {
        counteractionBlocked = true;
        blockedTurnsRemaining = turns;
        Debug.Log($"Counteraction blocked for {parent.emergencyType} SoE for {parent.player.playerName} for {turns} turns.");
    }

    // Process turn-based blocking
    public void TickTurn()
    {
        if (counteractionBlocked)
        {
            blockedTurnsRemaining--;
            if(blockedTurnsRemaining <= 0)
            {
                counteractionBlocked = false;
                Debug.Log($"Counteraction unblocked for {parent.emergencyType} SoE for {parent.player.playerName}.");
            }
        }
    }

    public void Deactivate()
    {
        isActive = false;
        tokensPut = 0;
        counteractionBlocked = false;
        blockedTurnsRemaining = 0;
        parent.Set(3);
        GameEvents.OnSoEDeactivated.Raise(new SoEContext(parent.emergencyType, parent.player));
    }
    public void RestoreBlockState(bool wasBlocked, int turnsRemaining)
    {
        counteractionBlocked = wasBlocked;
        blockedTurnsRemaining = turnsRemaining;
    }
}
