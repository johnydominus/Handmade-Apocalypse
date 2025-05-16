using UnityEngine;
using System.Linq;
#nullable enable

public class ApplyEffectCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController? player;

    public ApplyEffectCommand(Effect effect, PlayerController? player)
    {
        this.effect = effect;
        this.player = player;
    }

    public void Execute()
    {
        Debug.Log($"Applying effect: {effect.effectName} for {(player != null ? player.playerName : "global")}");

        switch (effect.effectTarget)
        {
            case EffectTarget.ThreatLevel:
                // Apply threat level effect
                GameServices.Instance.commandManager.ExecuteCommand(new ModifyThreatCommand(effect));
                break;

            case EffectTarget.EmergencyLevel:
                // Apply emergency level effect
                if (player != null)
                {
                    var emergencyType = EmergencyMapping.GetBySphere(effect.sphereType).emergency;

                    if(emergencyType != null)
                    {
                        var emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);

                        if (emergency != null)
                        {
                            if (effect.value > 0)
                                emergency.Increase(effect.value);
                            else
                                emergency.Decrease(effect.value);

                            Debug.Log($"Applied emergency change of {effect.value} to {emergencyType} for {player.playerName}");
                        }
                    }
                }
                break;

            case EffectTarget.SoE:
                // Handle State of Emergency effects
                if (player != null)
                {
                    var emergencyType = EmergencyMapping.GetBySphere(effect.sphereType).emergency;
                    if (emergencyType != null)
                    {
                        var emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                        if (emergency != null && emergency.stateOfEmergency != null)
                        {
                            if (effect.value > 0 && !emergency.stateOfEmergency.isActive)
                            {
                                emergency.stateOfEmergency.Activate();
                                Debug.Log($"Activated SoE for {emergencyType} in {player.playerName}'s region");
                            } 
                            else if (effect.value < 0 && emergency.stateOfEmergency.isActive) 
                            {
                                emergency.stateOfEmergency.Deactivate();
                                Debug.Log($"Deactivated SoE for {emergencyType} in {player.playerName}'s region");
                            }
                        }
                    }
                }
                break;

            case EffectTarget.ActivateThreat:
                Debug.Log("Applying ActivateThreat effect...");
                GameServices.Instance.commandManager.ExecuteCommand(new ActivateThreatCommand(effect));
                break;

            case EffectTarget.DeactivateThreat:
                Debug.Log("Applying DeactivateThreat effect...");
                GameServices.Instance.commandManager.ExecuteCommand(new DeactivateThreatCommand(effect));
                break;

            case EffectTarget.Dividends:
            case EffectTarget.General:
                // These effects are applied when their values are resolved
                Debug.Log($"Effect {effect.effectName} will be applied when its value is requested");
                break;

            default:
                Debug.LogWarning($"Unhandled effect target: {effect.effectTarget}");
                break;
        }
    }

    public void Undo()
    {
        Debug.Log($"Undoing effect: {effect.effectName}");

        // Implement undo logic based on effect type and target
        // For most effect types, this is complex and may require tracking
        // original values, so we might leave it unimplemented for now
    }
}
