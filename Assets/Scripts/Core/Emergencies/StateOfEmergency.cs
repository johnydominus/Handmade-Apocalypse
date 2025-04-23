using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StateOfEmergency
{
    List<Effect> effects = new();
    public bool isActive { get; private set; }
    public int tokensPut { get; private set; }

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
                EmergencyMapping.GetByEmergency(parent.emergencyType).sphere, 0,
                parent.player));
    }

    public void Activate()
    {
        isActive = true;
        GameEvents.OnSoEActivated.Raise(new SoEContext(parent.emergencyType, parent.player));
        Debug.Log($"⚠️ Emergency #{parent.emergencyType} activated!");
    }

    public void PutTokens(int amount)
    {
        tokensPut += amount;
        if (tokensPut >= tokensToDeactive)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        isActive = false;
        parent.Set(3);
    }
}
