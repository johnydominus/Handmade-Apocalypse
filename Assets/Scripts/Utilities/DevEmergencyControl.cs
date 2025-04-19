using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DevEmergencyControl : MonoBehaviour
{
    EmergencyType? emergencyType;
    DevTools devManager;

    public void Initialize(EmergencyType? emergency, DevTools devTools)
    {
        this.emergencyType = emergency;
        this.devManager = devTools;
    }

    public void IncreaseEmergency()
    {
        devManager.IncreaseEmergency(emergencyType);
    }

    public void DecreaseEmergency()
    {
        devManager.DecreaseEmergency(emergencyType);
    }
}
