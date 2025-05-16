using System.Collections.Generic;
using UnityEngine;

public class GlobalThreatTracker
{
    private HashSet<ThreatType> historicalThreats = new();
    private Dictionary<ThreatType, int> soeTurnCounter = new();
    private int maxSimultaneousThreats = 3;
    private bool maxThreatsEverReached = false;
    private List<ThreatType> activeThreats = new();

    public bool CanNewThreatAppear()
    {
        return !maxThreatsEverReached;
    }

    public void RegisterThreatActivation(ThreatType threat)
    {
        historicalThreats.Add(threat);

        if (!activeThreats.Contains(threat))
            activeThreats.Add(threat);
        
        if (activeThreats.Count > maxSimultaneousThreats)
        {
            maxThreatsEverReached = true;
            Debug.Log("Maximum number of threats reached! No new threats can appear.");
        }

        Debug.Log($"Registered threat activation: {threat}. Active threats: {activeThreats.Count}. Max reached: {maxThreatsEverReached}");
    }

    public void RegisterThreatDeactivation(ThreatType threat)
    {
        activeThreats.Remove(threat);
        Debug.Log($"Registered threat deactivation: {threat}. Active threats remaining: {activeThreats.Count}");
    }

    public void IncrementSoETurnCounter(ThreatType threatType)
    {
        if (!soeTurnCounter.ContainsKey(threatType))
            soeTurnCounter[threatType] = 0;

        soeTurnCounter[threatType]++;
        Debug.Log($"SoE turn counter for {threatType}: {soeTurnCounter[threatType]}");
   
        if (soeTurnCounter[threatType] >= 3 && CanNewThreatAppear())
        {
            // Create an effect to activate this threat
            Effect activationEffect = new Effect(
                EffectSource.SoE,
                EffectTarget.ActivateThreat,
                EffectType.Add,
                EmergencyMapping.GetByThreat(threatType).sphere,
                0,
                null);

            // Execute the effect through command system
            GameServices.Instance.commandManager.ExecuteCommand(
                new ActivateThreatCommand(activationEffect));
        }
    }

    public void ResetSoETurnCounter(ThreatType threatType)
    {
        if (soeTurnCounter.ContainsKey(threatType))
        {
            soeTurnCounter[threatType] = 0;
            Debug.Log($"Reset SoE turn counter for {threatType}");
        }   
    }

    public List<ThreatType> GetActiveThreats()
    {
        return activeThreats;
    }
}
