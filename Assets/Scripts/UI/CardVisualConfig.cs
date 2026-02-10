using System.Collections.Generic;
using UnityEngine;

public static class CardVisualConfig
{
    private static readonly Dictionary<CardPolarity, Color> polarityColors = new()
    {
        { CardPolarity.Positive, new Color(0.42f, 0.62f, 0.42f) },  // Muted green
        { CardPolarity.Negative, new Color(0.62f, 0.42f, 0.42f) },  // Muted red
        { CardPolarity.Neutral,  new Color(0.62f, 0.62f, 0.42f) }   // Muted yellow
    };

    private static readonly Dictionary<SphereType, Color> sphereColors = new()
    {
        { SphereType.Medicine,            new Color(0.36f, 0.54f, 0.54f) },  // Muted teal
        { SphereType.Diplomacy,           new Color(0.48f, 0.48f, 0.62f) },  // Muted lavender
        { SphereType.Agriculture,         new Color(0.48f, 0.62f, 0.48f) },  // Muted sage
        { SphereType.EducationAndScience, new Color(0.54f, 0.54f, 0.62f) },  // Muted steel
        { SphereType.Ecology,             new Color(0.42f, 0.54f, 0.48f) },  // Muted forest
        { SphereType.Astronautics,        new Color(0.36f, 0.42f, 0.54f) },  // Muted navy
        { SphereType.All,                 new Color(0.50f, 0.50f, 0.50f) },  // Default gray
        { SphereType.none,                new Color(0.50f, 0.50f, 0.50f) }   // Default gray
    };

    public static Color GetPolarityColor(CardPolarity polarity)
    {
        return polarityColors.TryGetValue(polarity, out var color) ? color : Color.black;
    }

    public static Color GetSphereColor(SphereType sphere)
    {
        return sphereColors.TryGetValue(sphere, out var color) ? color : new Color(0.50f, 0.50f, 0.50f);
    }
}
