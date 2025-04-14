using UnityEngine;

public class ThreatRaiseTest: MonoBehaviour
{
    public void RaisePandemic()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.Pandemic, +15);
    }

    public void KillHumanity()
    {
        GameServices.Instance.threatManager.ApplyThreatChange(ThreatType.Asteroid, +100);
    }
}
