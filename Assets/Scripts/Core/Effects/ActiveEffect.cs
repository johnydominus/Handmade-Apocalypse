using UnityEngine;

public class ActiveEffect
{
    public Effect effect;
    public PlayerController target;
    public EffectTiming timing;
    public bool isActive { get; private set; } = false;
    public bool expired = false;

    public ActiveEffect(Effect effect, PlayerController target)
    {
        this.effect = effect;
        this.target = target;
        this.timing = effect.effectTiming;
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
