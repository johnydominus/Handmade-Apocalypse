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

    // ── Effect summary generator ──────────────────────────────────────────

    public static string GenerateEffectSummary(List<Effect> effects)
    {
        var parts = new List<string>();
        foreach (var e in effects)
        {
            string part = DescribeEffect(e);
            if (!string.IsNullOrEmpty(part))
                parts.Add(part);
        }
        return string.Join("\n", parts);
    }

    private static string DescribeEffect(Effect e)
    {
        string valStr = e.effectType switch
        {
            EffectType.Add      => e.value >= 0 ? $"+{(int)e.value}" : $"{(int)e.value}",
            EffectType.Multiply => $"×{e.multiplierValue:0.#}",
            EffectType.Block    => "Block",
            _                   => ""
        };

        string targetStr = e.effectTarget switch
        {
            EffectTarget.ThreatLevel      => $"{valStr} {SphereToThreatLabel(e.sphereType)} Threat",
            EffectTarget.Dividends        => $"{valStr} {e.sphereType} Dividends",
            EffectTarget.EmergencyLevel   => $"{valStr} Emergency ({e.sphereType})",
            EffectTarget.PlayerTokens     => $"{valStr} Tokens",
            EffectTarget.ActivateThreat   => $"Activate {SphereToThreatLabel(e.sphereType)} Threat",
            EffectTarget.DeactivateThreat => $"Deactivate {SphereToThreatLabel(e.sphereType)} Threat",
            EffectTarget.SoE              => "State of Emergency",
            _                             => ""
        };

        if (string.IsNullOrEmpty(targetStr)) return "";

        // Append timing suffix
        var t = e.effectTiming;
        if (t != null)
        {
            targetStr += t.effectTimingType switch
            {
                EffectTimingType.MultiTurn  when t.duration > 0  => $" ({t.duration}T)",
                EffectTimingType.Delayed    when t.delay > 0 && t.duration > 0
                                                                => $" (delay {t.delay}T, {t.duration}T)",
                EffectTimingType.Delayed    when t.delay > 0     => $" (after {t.delay}T)",
                EffectTimingType.Recurring  when t.interval > 0  => $" (each {t.interval}T)",
                _                                                 => ""
            };
        }

        return targetStr;
    }

    private static string SphereToThreatLabel(SphereType sphere) => sphere switch
    {
        SphereType.Medicine     => "Pandemic",
        SphereType.Diplomacy    => "Nuclear War",
        SphereType.Astronautics => "Asteroid",
        _                       => sphere.ToString()
    };
}
