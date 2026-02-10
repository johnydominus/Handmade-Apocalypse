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

            case EffectConditionType.AllPlayersInvestedMinInSphere:
                return GameServices.Instance.turnManager.GetAllPlayers().All(p =>
                {
                    var slot = p.investments.FirstOrDefault(s => s.sphereName == targetSphere);
                    if (slot == null) return false;
                    int invested = slot.investors.TryGetValue(p, out var data) ? data.investedTokens : 0;
                    return invested >= requiredValue;
                });

            case EffectConditionType.AnySoEIsActive:
                if (player == null)
                {
                    // For Global Events, check all players
                    var allPlayers = GameServices.Instance.turnManager.GetAllPlayers();
                    return allPlayers.Any(p => p.emergencies.Any(e => e.stateOfEmergency != null && e.stateOfEmergency.isActive));
                }
                return player.emergencies.Any(e => e.stateOfEmergency != null && e.stateOfEmergency.isActive);

            case EffectConditionType.NoSoEIsActive:
                if (player == null)
                {
                    // For Global Events, check all players
                    var allPlayers = GameServices.Instance.turnManager.GetAllPlayers();
                    return !allPlayers.Any(p => p.emergencies.Any(e => e.stateOfEmergency != null && e.stateOfEmergency.isActive));
                }
                return !player.emergencies.Any(e => e.stateOfEmergency != null && e.stateOfEmergency.isActive);

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
    CardEffectIsActive,
    AllPlayersInvestedMinInSphere,
    AnySoEIsActive,
    NoSoEIsActive
}
