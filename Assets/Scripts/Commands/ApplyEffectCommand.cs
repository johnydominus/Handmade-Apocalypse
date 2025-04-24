using UnityEngine;

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
        switch (effect.effectTarget)
        {
            case EffectTarget.General:
                // Add logic to handle the General effect
                Debug.Log("Applying General effect");
                break;
            case EffectTarget.Dividends:
                // Add logic to handle the Dividends effect
                Debug.Log("Applying Dividends effect.");
                break;
            case EffectTarget.ThreatLevel:
                // Add logic to handle the ThreatLevel effect
                Debug.Log("Applying ThreatLevel effect");
                GameServices.Instance.commandManager.ExecuteCommand(new ModifyThreatCommand(effect));
                break;
            case EffectTarget.EmergencyLevel:
                // Add logic to handle the EmergencyLevel effect
                Debug.Log("Applying EmergencyLevel effect");
                break;
            case EffectTarget.SoE:
                // Add logic to hande the SoE effect
                Debug.Log("Applying SoE effect");
                break;
            default:
                Debug.LogWarning($"Unhandled effect target: {effect.effectTarget}");
                break;
        }
    }

    public void Undo()
    {

    }
}
