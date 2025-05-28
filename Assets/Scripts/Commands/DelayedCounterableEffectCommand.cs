using UnityEngine;

public class DelayedCounterableEffectCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private string registerEffectId;

    public DelayedCounterableEffectCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
    }

    public void Execute()
    {
        if (!effect.isCounterable)
        {
            Debug.LogWarning("Effect is not marked as counterable");
            return;
        }

        // Register the delayed effect
        GameServices.Instance.delayedCounteractionManager.RegisterDelayedEffect(effect, player);

        Debug.Log($"Registered Delayed Counterable Effect: {effect.counteractionDescription}");
    }

    public void Undo()
    {
        Debug.Log($"Undo requested for counterable effect {effect.counteractionDescription}");
        // TODO: Implement proper undo if needed
    }
}
