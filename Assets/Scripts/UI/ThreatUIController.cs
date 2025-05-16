using System.Linq;
using UnityEngine;

public class ThreatUIController : MonoBehaviour
{
    [SerializeField] private ThreatDisplay pandemicDisplay;
    [SerializeField] private ThreatDisplay nuclearWarDisplay;
    [SerializeField] private ThreatDisplay asteroidDisplay;

    private void OnEnable()
    {
        GameEvents.OnThreatChanged.RegisterListener(OnThreatChanged);
        GameEvents.OnThreatActivated.RegisterListener(OnThreatActivated);
        GameEvents.OnThreatDeactivated.RegisterListener(OnThreatDeactivated);


        // Initialize displays on enable
        UpdateAllThreatDisplays();
    }

    private void OnDisable()
    {
        GameEvents.OnThreatChanged.UnregisterListener(OnThreatChanged);
        GameEvents.OnThreatActivated.UnregisterListener(OnThreatActivated);
        GameEvents.OnThreatDeactivated.UnregisterListener(OnThreatDeactivated);
    }

    private void UpdateAllThreatDisplays()
    {
        // Get all activted threats
        var activeThreats = GameServices.Instance.threatManager.GetThreats();

        // Hide all displays first
        pandemicDisplay.gameObject.SetActive(false);
        nuclearWarDisplay.gameObject.SetActive(false);
        asteroidDisplay.gameObject.SetActive(false);

        // Show and update only active threats
        foreach (var threat in activeThreats)
        {
            ThreatDisplay display = GetDisplayForThreatType(threat.threatType);
            if (display != null)
            {
                display.gameObject.SetActive(true);
                display.SetValue(threat.threatValue, threat.threatDelta);
            }
        }
    }

    private void OnThreatActivated(ThreatType type)
    {
        // Get the threat value
        var threat = GameServices.Instance.threatManager.GetThreats().
            FirstOrDefault(t => t.threatType == type);

        if (threat == null)
            return;

        // Get the corresponding display and set it active
        ThreatDisplay display = GetDisplayForThreatType(type);
        if (display != null)
        {
            display.gameObject.SetActive(true);
            display.SetValue(threat.threatValue, threat.threatDelta);
        }
    }

    private void OnThreatDeactivated(ThreatType type)
    {
        ThreatDisplay display = GetDisplayForThreatType(type);
        if (display != null)
        {
            display.gameObject.SetActive(false);
        }
    }

    private void OnThreatChanged(ThreatType type)
    {
        var threat = GameServices.Instance.threatManager.GetThreats().
            FirstOrDefault(t => t.threatType == type);

        if (threat == null)
            return;

        int value = threat.threatValue;
        int delta = threat.threatDelta;

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

    private ThreatDisplay GetDisplayForThreatType(ThreatType type)
    {
        switch (type)
        {
            case ThreatType.Pandemic:
                return pandemicDisplay;
            case ThreatType.NuclearWar:
                return nuclearWarDisplay;
            case ThreatType.Asteroid:
                return asteroidDisplay;
            default:
                return null;
        }
    }
}
