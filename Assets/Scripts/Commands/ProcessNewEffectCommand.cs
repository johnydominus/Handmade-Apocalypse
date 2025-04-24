using UnityEngine;

public class ProcessNewEffectCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;

    public ProcessNewEffectCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
    }

    public void Execute()
    {
        if (!(effect.effectTiming.effectTimingType == EffectTimingType.Immediate))
            GameServices.Instance.effectManager.RegisterEffect(effect, player);

        if (!(effect.effectTiming.effectTimingType == EffectTimingType.Delayed))
            GameServices.Instance.effectManager.ApplyEffect(effect, player);
    }

    public void Undo()
    {

    }
}
