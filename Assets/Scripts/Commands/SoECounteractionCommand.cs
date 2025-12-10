using System.Linq;
using UnityEngine;

public class SoECounteractionCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private readonly int tokensAdded;
    private Emergency targetEmergency;

    public SoECounteractionCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
        this.tokensAdded = (int)effect.value;
    }

    public void Execute()
    {
        // Find the target emergency based on the effect's sphere type
        var emergencyType = EmergencyMapping.GetBySphere(effect.sphereType).emergency;
        if (emergencyType == null)
        {
            Debug.LogWarning($"No emergency found for sphere type {effect.sphereType}. Command will not execute.");
            return;
        }

        targetEmergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
        if (targetEmergency == null) 
        { 
            Debug.LogWarning($"Player {player.playerName} does not have an emergency of type {emergencyType}. Command will not execute.");
            return;
        }

        if (!targetEmergency.stateOfEmergency.isActive)
        {
            Debug.LogWarning($"Emergency {targetEmergency.emergencyType} is not active for {player.playerName}. Command will not execute.");
            return;
        }

        // Add counteraction tokens to the SoE
        targetEmergency.stateOfEmergency.PutTokens(tokensAdded);

        Debug.Log($"Added {tokensAdded} counteraction tokens to {emergencyType} SoE for {player.playerName}.");

        // Register effect if it has duration
        if (effect.effectTiming.effectTimingType != EffectTimingType.Immediate)
            GameServices.Instance.effectManager.RegisterEffect(effect, player);
    }

    public void Undo()
    {
        if (targetEmergency != null)
        {
            // Remove the tokens we added
            targetEmergency.stateOfEmergency.RemoveTokens(tokensAdded);
            Debug.Log($"Removed {tokensAdded} counteraction tokens from {targetEmergency.emergencyType} SoE for {player.playerName}.");
        }
    }
}


