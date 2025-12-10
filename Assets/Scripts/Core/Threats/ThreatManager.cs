using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatManager
{
    public GlobalThreatTracker threatTracker { get; private set; } = new();
    private List<Threat> threats = new();
    BuildType buildType;
    
    public void Initialize(List<ThreatType> threatList, BuildType bType)
    {
        Debug.Log($"Build type is {buildType.ToString()}");
        buildType = bType;

        switch (buildType)
        {
            case BuildType.FullGame:
                InitializeThreatsAndStartOne(threatList);
                // Additional functionality can be added here for full game
                break;

            case BuildType.BasicPrototype:
                InitializeThreatsAndStartOne(threatList);
                // Additional functionality can be added here for basic prototype
                break;

            case BuildType.AdvancedPrototype:
                InitializeThreatsAndStartOne(threatList);
                // Additional functionality can be added here for advanced prototype
                break;
        }

        foreach (var threat in threats)
        {
            GameEvents.OnThreatActivated.Raise(threat.threatType);
            GameEvents.OnThreatChanged.Raise(threat.threatType);
        }
    }

    public void ApplyThreatChange(ThreatType type, int amount)
    {
        if (!threats.Any(e => e.threatType == type)) return;

        var threat = threats.First(e => e.threatType == type);
        int oldValue = threat.threatValue;
        threat.threatValue = Mathf.Clamp(threat.threatValue + amount, 0, 100);
        threat.threatDelta = amount; // Store the delta for UI updates

        Debug.Log($"Threat {type} changed by {amount}. New value: {threat.threatValue}");

        GameEvents.OnThreatChanged.Raise(type);

        if (threat.threatValue <= 0)
        {
            DeactivateThreat(type);
        }

        CheckWinLossConditions();
    }

    public int GetThreatValue(ThreatType type)
    {
        var match = threats.FirstOrDefault(e => e.threatType == type);
        return match != null ? match.threatValue : -1;
    }


    public void CheckWinLossConditions()
    {
        bool allZero = true;

        foreach (var threat in threats)
        {
            if (threat.threatValue > 0)
            {
                allZero = false;
                break;
            }
        }

        if (allZero)
        {
            GameEvents.OnVictory.Raise();
            return;
        }

        foreach (var threat in threats)
        {
            if (threat.threatValue >= 100)
            {
                GameEvents.OnLoss.Raise(threat.threatType);
            }
        }
    }

    public List<Threat> GetThreats()
    {
        return threats;
    }

    public void ActivateThreat(ThreatType threatType)
    {
        // Check if threat already exists
        if (threats.Any(t => t.threatType == threatType))
        {
            Debug.Log($"Threat {threatType} already exists. No action taken.");
            return;
        }

        // Check if we've reached the max threats limit
        if (!threatTracker.CanNewThreatAppear())
        {
            Debug.Log($"Cannot activate threat {threatType} due to max threats limit.");
            return;
        }

        // Create and add the new threat (starting at 50)
        Threat newThreat = new Threat(threatType, 50);
        threats.Add(newThreat);

        // Register activation with tracker
        threatTracker.RegisterThreatActivation(threatType);

        // Raise event
        GameEvents.OnThreatActivated.Raise(threatType);
        Debug.Log($"Activated threat: {threatType}");
    }

    public void DeactivateThreat(ThreatType threatType)
    {
        var threat = threats.FirstOrDefault(t => t.threatType == threatType);
        if (threat != null)
        {
            threats.Remove(threat);

            // Register the deactivattion with the tracker
            threatTracker.RegisterThreatDeactivation(threatType);

            GameEvents.OnThreatDeactivated.Raise(threatType);
            Debug.Log($"Deactivated threat: {threatType}");

            if (threats.Count == 0)
            {
                GameEvents.OnVictory.Raise();
            }
        }
    }

    public void InitializeThreatsAndStartOne(List<ThreatType> threatList)
    {
        if (threatList.Count > 0)
        {
            // Select a random threat
            int randomIndex = UnityEngine.Random.Range(0, threatList.Count);
            ThreatType randomThreat = threatList[randomIndex];

            // Directly add the theat without using commands
            // (since we're still in initialization)
            threats.Add(new Threat(randomThreat, 50));
            threatTracker.RegisterThreatActivation(randomThreat);

            Debug.Log($"Initialized with single random threat: {randomThreat}");
        }
    }

    public void StartAllThreatsAtValue(List<ThreatType> threatList, int value)
    {
        foreach (var threat in threatList)
        {
            threats.Add(new Threat(threat, value));
            // Register with tracker
            threatTracker.RegisterThreatActivation(threat);
            Debug.Log($"Started threat: {threat}");
        }
    }
}
