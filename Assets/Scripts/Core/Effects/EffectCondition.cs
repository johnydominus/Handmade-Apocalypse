using System.Linq;
using UnityEngine;

[System.Serializable]
public class EffectCondition
{
    public EffectConditionType conditionType;
    public SphereType targetSphere = SphereType.All;
    public int requiredValue;
    public string targetCardName = "";

    public bool CheckCondition(PlayerController player)
    {
        switch (conditionType)
        {
            case EffectConditionType.PlayerHasMinTokens:
                return player.tokenManager.GetTokens() >= requiredValue;

            case EffectConditionType.PlayerHasMaxTokens:
                return player.tokenManager.GetTokens() <= requiredValue;

            case EffectConditionType.ThreatAtLeast:
                var threatType = EmergencyMapping.GetBySphere(targetSphere).threat;
                return GameServices.Instance.threatManager.GetThreatValue(threatType) >= requiredValue;

            case EffectConditionType.ThreatAtMost:
                threatType = EmergencyMapping.GetBySphere(targetSphere).threat;
                return GameServices.Instance.threatManager.GetThreatValue(threatType) <= requiredValue;

            case EffectConditionType.EmergencyAtLeast:
                var emergencyType = EmergencyMapping.GetBySphere(targetSphere).emergency;
                var emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                return emergency != null && emergency.emergencyLevel >= requiredValue;

            case EffectConditionType.EmergencyAtMost:
                emergencyType = EmergencyMapping.GetBySphere(targetSphere).emergency;
                emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                return emergency == null || emergency.emergencyLevel <= requiredValue;

            case EffectConditionType.SoEIsActive:
                emergencyType = EmergencyMapping.GetBySphere(targetSphere).emergency;
                emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                return emergency != null && emergency.stateOfEmergency.isActive;

            case EffectConditionType.SoEIsInactive:
                emergencyType = EmergencyMapping.GetBySphere(targetSphere).emergency;
                emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType);
                return emergency != null || !emergency.stateOfEmergency.isActive;

            case EffectConditionType.CardEffectIsActive:
                var activeEffects = GameServices.Instance.effectManager.GetActiveEffects();
                return activeEffects.Any(e => e.effect.sourceCardName == targetCardName && e.isActive);

            default:
                return true; // Default to true for unknown conditions
        }
    }
}

public enum EffectConditionType
{
    None,
    PlayerHasMinTokens,
    PlayerHasMaxTokens,
    ThreatAtLeast,
    ThreatAtMost,
    EmergencyAtLeast,
    EmergencyAtMost,
    SoEIsActive,
    SoEIsInactive,
    CardEffectIsActive
}
