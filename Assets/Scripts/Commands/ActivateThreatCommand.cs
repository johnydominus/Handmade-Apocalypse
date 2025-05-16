using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ActivateThreatCommand : ICommand
{
    private readonly Effect effect;
    private bool wasActivated = false;

    public ActivateThreatCommand(Effect effect)
    {
        this.effect = effect;
    }

    public void Execute()
    {
        // Determine the threat type based on the sphere type
        ThreatType threatToActivate = EmergencyMapping.GetBySphere(effect.sphereType).threat;

        // Activate the threat via ThreatManager
        GameServices.Instance.threatManager.ActivateThreat(threatToActivate);

        // Register activation (for Undo)
        wasActivated = true;
    }

    public void Undo()
    {
        if (wasActivated)
        {
            // If we need to undo, just use DeactivateThreatCommand logic
            ThreatType threatToDeactivate = EmergencyMapping.GetBySphere(effect.sphereType).threat;
            GameServices.Instance.threatManager.DeactivateThreat(threatToDeactivate);
        }
    }
}
