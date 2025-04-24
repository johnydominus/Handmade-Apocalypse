using NUnit.Framework;
using UnityEngine;

public class ModifyThreatCommand : ICommand
{
    private readonly Effect effect;
    private int originalThreatLevel;

    public ModifyThreatCommand(Effect effect)
    {
        this.effect = effect;
    }

    public void Execute()
    {
        var threatManager = GameServices.Instance.threatManager;
        var targetThreat = EmergencyMapping.GetBySphere(effect.sphereType).threat;

        originalThreatLevel = threatManager.GetThreatValue(targetThreat);
        threatManager.ApplyThreatChange(targetThreat, effect.value);
    }

    public void Undo()
    {
        var threatManager = GameServices.Instance.threatManager;
        var targetThreat = EmergencyMapping.GetBySphere(effect.sphereType).threat;

        int delta = originalThreatLevel - threatManager.GetThreatValue(targetThreat);
        threatManager.ApplyThreatChange(targetThreat, delta);
    }
}
