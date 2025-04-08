using System.Collections.Generic;
using UnityEngine;

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
