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
        Debug.Log($"Modifying threat by {effect.value} for {effect.sphereType}");
        var threatManager = GameServices.Instance.threatManager;

        if (effect.sphereType == SphereType.All)
        {
            foreach (var entry in EmergencyMapping.GetAll())
            {
                threatManager.ApplyThreatChange(entry.threat, (int)effect.value);
            }
            return;
        }

        var mapping = EmergencyMapping.GetBySphere(effect.sphereType);
        if (mapping == null)
        {
            Debug.LogError($"No mapping found for sphere type {effect.sphereType}");
            return;
        }

        originalThreatLevel = threatManager.GetThreatValue(mapping.threat);
        threatManager.ApplyThreatChange(mapping.threat, (int)effect.value);
    }

    public void Undo()
    {
        var threatManager = GameServices.Instance.threatManager;
        var targetThreat = EmergencyMapping.GetBySphere(effect.sphereType).threat;

        int delta = originalThreatLevel - threatManager.GetThreatValue(targetThreat);
        threatManager.ApplyThreatChange(targetThreat, delta);
    }
}
