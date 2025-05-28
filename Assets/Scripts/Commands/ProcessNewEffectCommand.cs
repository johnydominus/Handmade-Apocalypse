using UnityEngine;

public class ProcessNewEffectCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private readonly CardData sourceCard;       // Add source card reference

    public ProcessNewEffectCommand(Effect effect, PlayerController player, CardData sourceCard = null)
    {
        this.effect = effect;
        this.player = player;
        this.sourceCard = sourceCard;
    }

    public void Execute()
    {
        // Set source tracking information
        if (sourceCard != null)
        {
            effect.sourceCardName = sourceCard.cardName;
            effect.sourceCardType = sourceCard.cardType;
        }

        // Handle counter effects
        if (effect.hasCondition)
        {
            bool conditionMet = effect.condition.CheckCondition(player);

            if (conditionMet)
            {
                Debug.Log($"Condition met for effect {effect.effectName}. Applying effect.");
                ApplyNormalEffect();
            }
            else if (effect.alternativeEffects.Count > 0)
            {
                Debug.Log($"Condition not met for effect {effect.effectName}. Applying {effect.alternativeEffects.Count} alternative effects.");

                // Apply alternative effects
                foreach (var altEffect in effect.alternativeEffects)
                {
                    ICommand altCommand = new ProcessNewEffectCommand(altEffect, player, sourceCard);
                    GameServices.Instance.commandManager.ExecuteCommand(altCommand);
                }
            }
            else
            {
                Debug.Log($"Condition not met for effect {effect.effectName}, effect skipped");
            }
            return;
        }

        // Apply normal effect if no condition
        ApplyNormalEffect();
    }

    private void ApplyNormalEffect()
    {
        // Handle special effect targets first
        if (effect.effectTarget == EffectTarget.CounterCardEffect)
        {
            GameServices.Instance.commandManager.ExecuteCommand(new CounterCardEffectCommand(effect, player));
            return;
        }
        
        if (effect.effectTarget == EffectTarget.SoECounterAction)
        {
            GameServices.Instance.commandManager.ExecuteCommand(new SoECounteractionCommand(effect, player));
            return;
        }

        if (effect.effectTarget == EffectTarget.SoEBlock)
        {
            GameServices.Instance.commandManager.ExecuteCommand(new SoEBlockCommand(effect, player));
            return;
        }

        if (effect.effectTarget == EffectTarget.DelayedCounterable)
        {
            GameServices.Instance.commandManager.ExecuteCommand(new DelayedCounterableEffectCommand(effect, player));
            return;
        }

        // Original logic for other effect targets
        if (!(effect.effectTiming.effectTimingType == EffectTimingType.Immediate))
            GameServices.Instance.effectManager.RegisterEffect(effect, player);

        if (!(effect.effectTiming.effectTimingType == EffectTimingType.Delayed))
            GameServices.Instance.effectManager.ApplyEffect(effect, player);
    }

    public void Undo()
    {

    }
}
