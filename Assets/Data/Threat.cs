using UnityEngine;

public class Threat
{
    public string threatName;
    public int threatValue; // 0-100
    public int threatDelta; // optional change this turn

    public Threat(string name, int value = 50)
    {
        threatName = name;
        threatValue = value;
        threatDelta = 0;
    }

    public void Increase(int amount)
    {
        threatValue = Mathf.Clamp(threatValue + amount, 0, 100);
        threatDelta = amount;
    }

    public void Decrease(int amount)
    {
        threatValue = Mathf.Clamp(threatValue - amount, 0, 100);
        threatDelta = -amount;
    }

    public void ResetDelta()
    {
        threatDelta = 0;
    }
}
