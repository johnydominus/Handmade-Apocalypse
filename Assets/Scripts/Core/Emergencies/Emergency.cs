using UnityEngine;

public class Emergency
{
    public EmergencyType? emergencyType { get; private set; }
    public PlayerController player { get; private set; }
    public StateOfEmergency stateOfEmergency { get; private set; }
    public int emergencyLevel { get; private set; }
    public int emergencyDelta;

    public Emergency(EmergencyType? type, PlayerController thePlayer, int level)
    {
        this.emergencyType = type;
        this.player = thePlayer;
        this.emergencyLevel = level;
        this.stateOfEmergency = new(this);
    }

    public void Increase(int amount)
    {
        if (stateOfEmergency.isActive) return;

        emergencyLevel = Mathf.Clamp(emergencyLevel + amount, 0, 10);
        emergencyDelta = amount;

        if (emergencyLevel >= 10)
            stateOfEmergency.Activate();
    }

    public void Decrease(int amount)
    {
        emergencyLevel = Mathf.Clamp(emergencyLevel - amount, 0, 10);
        emergencyDelta = -amount;
    }

    public void Set(int amount)
    {
        emergencyLevel = Mathf.Clamp(amount, 0, 10);
    }

    public void ResetDelta()
    {
        emergencyDelta = 0;
    }
}
