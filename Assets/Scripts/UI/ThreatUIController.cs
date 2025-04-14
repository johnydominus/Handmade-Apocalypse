using UnityEngine;

public class ThreatUIController : MonoBehaviour
{
    [SerializeField] private ThreatDisplay pandemicDisplay;
    [SerializeField] private ThreatDisplay nuclearWarDisplay;
    [SerializeField] private ThreatDisplay asteroidDisplay;

    private void OnEnable()
    {
        GameEvents.OnThreatChanged.RegisterListener(OnThreatChanged);
    }

    private void OnDisable()
    {
        GameEvents.OnThreatChanged.UnregisterListener(OnThreatChanged);
    }

    private void OnThreatChanged(ThreatType type)
    {
        int value = GameServices.Instance.threatManager.GetThreatValue(type);

        switch(type)
        {
            case ThreatType.Pandemic:
                pandemicDisplay.SetValue(value);
                break;
            case ThreatType.NuclearWar:
                nuclearWarDisplay.SetValue(value);
                break;
            case ThreatType.Asteroid:
                asteroidDisplay.SetValue(value);
                break;
        }
    }
}
