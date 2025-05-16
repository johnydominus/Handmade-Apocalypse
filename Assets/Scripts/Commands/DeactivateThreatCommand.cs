using System.Linq;
using UnityEngine;

public class DeactivateThreatCommand : ICommand
{
    private readonly Effect effect;
    private ThreatType? deactivatedThreat = null;
    private int previousThreatValue = 0;

    public DeactivateThreatCommand(Effect effect)
    {
        this.effect = effect;
    }

    public void Execute()
    {
        // Determine the threat type to deactivate based on the sphere type
        ThreatType threatToDeactivate = EmergencyMapping.GetBySphere(effect.sphereType).threat;

        // Find the threat if it exists
        var threat = GameServices.Instance.threatManager.GetThreats()
            .FirstOrDefault(t => t.threatType == threatToDeactivate);

        if (threat != null)
        {
            // Save state for possible undo
            deactivatedThreat = threat.threatType;
            previousThreatValue = threat.threatValue;

            // Deactivate the threat using the ThreatManager
            GameServices.Instance.threatManager.DeactivateThreat(threatToDeactivate);

        }
        else
        {
            Debug.Log($"Threat {threatToDeactivate} not found to be deactivated. No action taken.");
        }
    }

    public void Undo()
    {
        if (deactivatedThreat.HasValue)
        { 
            // For undo we need to re-activate the threat and restore its value
            // First activate it (will create with value 50)
            GameServices.Instance.threatManager.ActivateThreat(deactivatedThreat.Value);

            // Then adjust the value if needed
            if (previousThreatValue != 50)
            {
                int adjustment = previousThreatValue - 50;
                GameServices.Instance.threatManager.ApplyThreatChange(deactivatedThreat.Value, adjustment);
            }
            

            Debug.Log($"Restored threat: {deactivatedThreat.Value} (Undo operation)");
        }
    }
}
