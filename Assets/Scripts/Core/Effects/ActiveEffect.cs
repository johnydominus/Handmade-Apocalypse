using UnityEngine;
#nullable enable

public class ActiveEffect
{
    public Effect effect;
    public PlayerController? player;
    public EffectTiming timing;
    public bool isActive = false;
    public bool expired = false;

    public ActiveEffect(Effect effect, PlayerController? player)
    {
        this.effect = effect.Clone();
        this.player = player;
        this.timing = new EffectTiming
        {
            effectTimingType = effect.effectTiming.effectTimingType,
            duration = effect.effectTiming.duration,
            delay = effect.effectTiming.delay,
            interval = effect.effectTiming.interval,
            turnsElapsed = 0
        };

        // Immediate and MultiTurn effects are active immediately
        if (effect.effectTiming.effectTimingType == EffectTimingType.Immediate ||
            effect.effectTiming.effectTimingType == EffectTimingType.MultiTurn ||
            effect.effectTiming.effectTimingType == EffectTimingType.CurrentTurn)
        {
            Activate();
        }
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log($"Effect {effect.effectName} activated" + (player != null ? $" for {player.playerName}" : ""));
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log($"Effect {effect.effectName} deactivated" + (player != null ? $" for {player.playerName}" : ""));
    }

    public override string ToString()
    {
        string playerInfo = player != null ? $" for {player.playerName}" : " (global)";
        string timingInfo = $"[{timing.effectTimingType}, {timing.turnsElapsed}/{timing.duration}]";
        string statusInfo = isActive ? "Active" : expired ? "Expired" : "Inactive";

        return $"{effect.effectName}{playerInfo} - {timingInfo} - {statusInfo}";
    }
}
