using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayedCounterableEffect
{
    public string effectId;     // Unique identifier for the effect
    public string description;
    public int turnsRemaining;
    public int tokensRequired;
    public int tokensContributed;
    public List<Effect> effectsToApply;
    public bool isCountered;
    public bool hasTriggered;

    // Track which players conributed (for potential refunds)
    public Dictionary<string, int> playerContributions = new();

    public DelayedCounterableEffect(string id, string desc, int turns, int tokensNeeded, List<Effect> effects)
    {
        effectId = id;
        description = desc;
        turnsRemaining = turns;
        tokensRequired = tokensNeeded;
        tokensContributed = 0;
        effectsToApply = new List<Effect>(effects);
        isCountered = false;
        hasTriggered = false;
    }

    public bool AddTokens(PlayerController player, int amount)
    {
        if (isCountered || hasTriggered)
        {
            Debug.Log($"Cannot add tokens to {effectId} - already resolved");
            return false;
        }

        if (!player.tokenManager.HasEnoughTokens(amount)) 
        {
            Debug.Log($"{player.playerName} doesn't have enough tokens to contribute {amount}");
            return false;
        }

        // Spend the tokens
        player.tokenManager.SpendTokens(amount);

        // Track contribution
        if (!playerContributions.ContainsKey(player.playerName))
            playerContributions[player.playerName] = 0;

        playerContributions[player.playerName] += amount;
        tokensContributed += amount;

        Debug.Log($"{player.playerName} contributed {amount} tokens to counter '{description}'. " +
                 $"Total: {tokensContributed}/{tokensRequired}");

        // Check if countered
        if (tokensContributed >= tokensRequired)
        {
            isCountered = true;
            Debug.Log($"Effect {effectId} has been successfullly countered!");
            GameEvents.OnDelayedEffectCountered.Raise(this);
        }

        GameEvents.OnDelayedEffectUpdated.Raise(this);
        return true;
    }

    public void TicKTurn()
    {
        if (isCountered || hasTriggered)
            return;

        turnsRemaining--;
        Debug.Log($"Delayed effect '{description}' - {turnsRemaining} turns remaining");

        if (turnsRemaining <= 0)
            TriggerEffect();
        else
            GameEvents.OnDelayedEffectUpdated.Raise(this);
    }

    public void TriggerEffect()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (isCountered)
        {
            Debug.Log($"Delayed effect '{description}' was countered - no negative effects applied");
        }
        else
        {
            Debug.Log($"Delayed effect '{description}' was not countered - applying negative effects");

            // Apply all the negative effects
            foreach (var effect in effectsToApply)
            {
                // Apply to all players or specific targets based on effect design
                if (effect.sphereType == SphereType.All)
                {
                    // Apply to all players
                    foreach (var player in GameServices.Instance.turnManager.GetAllPlayers())
                    {
                        GameServices.Instance.commandManager.ExecuteCommand(
                            new ProcessNewEffectCommand(effect, player));
                    }
                }
                else
                {
                    // Apply globally of to current player base on effect type
                    GameServices.Instance.commandManager.ExecuteCommand(
                        new ProcessNewEffectCommand(effect, null));
                }
            }
        }

        GameEvents.OnDelayedEffectTriggered.Raise(this);
    }
}
