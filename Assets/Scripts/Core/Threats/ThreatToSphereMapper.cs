using System.Collections.Generic;
using UnityEngine;

public static class ThreatToSphereMapper
{
    private static readonly Dictionary<ThreatType, string> threatToSphereName = new Dictionary<ThreatType, string>
    {
        { ThreatType.Pandemic, "Medicine" },
        { ThreatType.NuclearWar, "Diplomacy" },
        { ThreatType.ClimateChange, "Ecology" },
        { ThreatType.Asteroid, "Astronomy" },
        { ThreatType.Hunger, "Agriculture" },
        { ThreatType.DarkAges, "Education" }
    };

    public static List<string> GetSphereNames(List<ThreatType> enabledThreats)
    {
        return enabledThreats.ConvertAll(threat => threatToSphereName[threat]);
    }
}
