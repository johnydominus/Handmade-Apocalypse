using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatManager
{
    private List<Threat> threats = new();
    
    public void Initialize(List<ThreatType> threatList, BuildType buildType)
    {
        switch (buildType)
        {
            case BuildType.BasicPrototype:
                foreach (var threat in threatList)
                {
                    threats.Add(new Threat(threat, 50));
                }
                break;
            case BuildType.AdvancedPrototype:
                // Add functionality if needed
                break;
            case BuildType.FullGame:
                if (threatList.Count == 0) return;

                ThreatType randomThreat = threatList[UnityEngine.Random.Range(0, threatList.Count)];
                threats.Add(new Threat(randomThreat, 50));

                // Show the message
                // return on button click
                break;
        } 
    }

    public void ApplyThreatChange(ThreatType type, int amount)
    {
        if (!threats.Any(e => e.threatType == type)) return;

        var threat = threats.First(e => e.threatType == type);
        threat.threatValue = Mathf.Clamp(threat.threatValue + amount, 0, 100);

        Debug.Log($"Threat {type} changed by {amount}. New value: {threat.threatValue}");

        GameEvents.OnThreatChanged.Raise(type);
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
                GameEvents.OnLoss.Raise();
            }
        }
    }

    public List<Threat> GetThreats()
    {
        return threats;
    }
}
