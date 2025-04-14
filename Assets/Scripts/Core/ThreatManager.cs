using System.Collections.Generic;
using UnityEngine;

public class ThreatManager
{
    private Dictionary<ThreatType, Threat> threats = new Dictionary<ThreatType, Threat>();
    
    public void Initialize()
    {
        threats[ThreatType.Pandemic] = new Threat("Pandemic", 50);
        threats[ThreatType.NuclearWar] = new Threat("Nuclear War", 50);
        threats[ThreatType.Asteroid] = new Threat("Asteroid", 50);
        //threats[ThreatType.Hunger] = new Threat("Hunger", 50);
        //threats[ThreatType.DarkAges] = new Threat("DarkAges", 50);
        //threats[ThreatType.ClimateChange] = new Threat("Climate Change", 50);
    }

    public void ApplyThreatChange(ThreatType type, int amount)
    {
        if (!threats.ContainsKey(type)) return;

        var threat = threats[type];
        threat.threatValue = Mathf.Clamp(threat.threatValue + amount, 0, 100);

        Debug.Log($"Threat {type} changed by {amount}. New value: {threat.threatValue}");

        GameEvents.OnThreatChanged.Raise(type);
        CheckWinLossConditions();
    }

    public int GetThreatValue(ThreatType type)
    {
        return threats.ContainsKey(type) ? threats[type].threatValue : -1;
    }

    public void CheckWinLossConditions()
    {
        bool allZero = true;
        foreach (var threat in threats.Values)
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

        foreach (var threat in threats.Values)
        {
            if (threat.threatValue >= 100)
            {
                GameEvents.OnLoss.Raise();
            }
        }
    }
}

/* OLD IMPLEMENTATION
public class ThreatManager : MonoBehaviour
{
    public List<ThreatUI> threatUIs; // Assigned in inspector
    private List<Threat> threats;

    void Start()
    {
        threats = new List<Threat>
        {
            new Threat("Pandemic", 50),
            new Threat("Nuclear War", 50),
            new Threat("Asteroid", 50)
        };

        UpdateThreatUI();
    }

    public void IncreaseThreat(int index, int amount)
    {
        threats[index].Increase(amount);
        UpdateThreatUI();
    }

    public void DecreaseThreat(int index, int amount)
    {
        threats[index].Decrease(amount);
        UpdateThreatUI();
    }

    public void ApplyThreatChange(ThreatType type, int amount)
    {
        int index = (int)type;
        if (index >= 0 && index < threats.Count)
        {
            threats[index].threatValue = Mathf.Clamp(threats[index].threatValue + amount, 0, 100);
            threats[index].threatDelta = 0; // Reset forecast or recalculate if needed
            UpdateThreatUI();

            Debug.Log($"Threat '{type}' changed by {amount}.");
        }
        else
        {
            Debug.LogWarning($"Invalid threat index: {index} for {type}");
        }
    }

    public void UpdateThreatUI()
    {
        for (int i = 0; i < threats.Count; i++)
        {
            threatUIs[i].UpdateDisplay(threats[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            IncreaseThreat(0, Random.Range(2, 6));
        }
    }
}
*/