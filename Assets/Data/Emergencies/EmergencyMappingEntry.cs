using UnityEngine;

public class EmergencyMappingEntry
{
    public ThreatType threat;
    public EmergencyType? emergency;
    public SphereType sphere;

    public EmergencyMappingEntry(ThreatType threat, EmergencyType? emergency, SphereType sphere)
    {
        this.threat = threat;
        this.emergency = emergency;
        this.sphere = sphere;
    }
}

