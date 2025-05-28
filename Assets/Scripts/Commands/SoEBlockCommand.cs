using System.Linq;
using UnityEngine;

public class SoEBlockCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private Emergency targetEmergency;
    private bool wasBlocked = false;
    private int previousBlockedTurns = 0;

    public SoEBlockCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
    }

    public void Execute() 
    {
        // Find the target emergency
        var emergencyType = EmergencyMapping.GetBySphere(effect.sphereType).emergency;
        if (emergencyType == null)
        {
            Debug.LogWarning($"No emergency found for sphere {effect.sphereType}.");
            return;
        }

        targetEmergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
        if (targetEmergency == null || !targetEmergency.stateOfEmergency.isActive)
        {
            Debug.LogWarning($"No active SoE {emergencyType} found for {player.playerName}.");
            return;
        }

        // Store previous state for undo functionality
        wasBlocked = targetEmergency.stateOfEmergency.counteractionBlocked;
        previousBlockedTurns = targetEmergency.stateOfEmergency.blockedTurnsRemaining;

        // Block counteraction
        int blockTurns = effect.value > 0 ? effect.value : 1; // Ensure at least 1 turn of blocking
        targetEmergency.stateOfEmergency.BlockCounteractionForTurns(blockTurns);

        Debug.Log($"{player.playerName} blocked counteraction for {targetEmergency.emergencyType} for {blockTurns} turns.");

        // Register effect if it has duration
        if (effect.effectTiming.effectTimingType != EffectTimingType.Immediate)
        {
            GameServices.Instance.effectManager.RegisterEffect(effect, player);            
        }
    }

    public void Undo() 
    { 
        if (targetEmergency != null)
        {
            // Restore previous state
            targetEmergency.stateOfEmergency.RestoreBlockState(wasBlocked, previousBlockedTurns);
            Debug.Log($"{player.playerName} undid the block on counteraction for {targetEmergency.emergencyType}.");
        }
    }
}
