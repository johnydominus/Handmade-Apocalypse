using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public static class EmergencyMapping
{
    private static readonly List<EmergencyMappingEntry> entries = new()
    {
        new(ThreatType.Pandemic, EmergencyType.Epidemic, SphereType.Medicine),
        new(ThreatType.NuclearWar, EmergencyType.MartialLaw, SphereType.Diplomacy),
        new(ThreatType.ClimateChange, EmergencyType.EcologicalCrisis, SphereType.Ecology),
        new(ThreatType.Hunger, EmergencyType.FoodCrisis, SphereType.Agriculture),
        new(ThreatType.DarkAges, EmergencyType.EducationalCrisis, SphereType.EducationAndScience),
        new(ThreatType.Asteroid, null, SphereType.Astronautics)
    };

    public static EmergencyMappingEntry GetByThreat(ThreatType threatType) => entries.First(e => e.threat == threatType);
    public static EmergencyMappingEntry GetByEmergency(EmergencyType? emergencyType) => entries.First(e => e.emergency == emergencyType);
    public static EmergencyMappingEntry GetBySphere(SphereType sphereType) => entries.First(e => e.sphere == sphereType);
    public static List<EmergencyMappingEntry> GetAll() => entries;

    public static List<SphereType> GetSphereTypesByThreat(List<ThreatType> threatTypes)
    {
        return threatTypes.Select(GetByThreat).Select(e => e.sphere).ToList();
    }

    public static List<EmergencyType?> GetEmergencyTypesByThreat(List<ThreatType> threatTypes)
    {
        return threatTypes.Select(GetByThreat).Select(e => e.emergency).ToList();
    }
}
