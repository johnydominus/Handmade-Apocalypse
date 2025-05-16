using System.Linq;
using UnityEngine;

public class ThreatRaiseTest: MonoBehaviour
{
    public void RaisePandemic()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.Pandemic, +15);
    }

    public void KillHumanity()
    {
        if (GameServices.Instance.threatManager.threatTracker.GetActiveThreats().Any(t => t == ThreatType.Asteroid))
            GameServices.Instance.threatManager.ActivateThreat(ThreatType.Asteroid);

        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.Asteroid, +100);
    }

    public void SaveHumanity()
    {
        var threats = GameServices.Instance.threatManager.threatTracker.GetActiveThreats();

        foreach (var threat in threats)
        {
            GameServices.Instance.threatManager.DeactivateThreat(threat);
        }
    }

    public void DecreasePandemic()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.Pandemic, -10);
    }

    public void IncreaseNuclearWar()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.NuclearWar, +10);
    }

    public void DecreaseNuclearWar()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.NuclearWar, -10);
    }
}
