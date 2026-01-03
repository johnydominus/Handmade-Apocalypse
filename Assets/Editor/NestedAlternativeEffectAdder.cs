#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class NestedAlternativeEffectAdder
{
    // Add alternative to the first alternative effect of main Effect 0
    [MenuItem("Tools/Cards/Add Alternative To Alt0 of Effect 0")]
    public static void AddToAlt0OfEffect0() => AddAlternativeToAlternativeEffect(0, 0);

    [MenuItem("Tools/Cards/Add Alternative To Alt0 of Effect 1")]
    public static void AddToAlt0OfEffect1() => AddAlternativeToAlternativeEffect(1, 0);

    [MenuItem("Tools/Cards/Add Alternative To Alt0 of Effect 2")]
    public static void AddToAlt0OfEffect2() => AddAlternativeToAlternativeEffect(2, 0);

    [MenuItem("Tools/Cards/Add Alternative To Alt0 of Effect 3")]
    public static void AddToAlt0OfEffect3() => AddAlternativeToAlternativeEffect(3, 0);

    [MenuItem("Tools/Cards/Add Alternative To Alt1 of Effect 0")]
    public static void AddToAlt1OfEffect0() => AddAlternativeToAlternativeEffect(0, 1);

    [MenuItem("Tools/Cards/Add Alternative To Alt1 of Effect 1")]
    public static void AddToAlt1OfEffect1() => AddAlternativeToAlternativeEffect(1, 1);

    [MenuItem("Tools/Cards/Add Alternative To Alt1 of Effect 2")]
    public static void AddToAlt1OfEffect2() => AddAlternativeToAlternativeEffect(2, 1);

    [MenuItem("Tools/Cards/Add Alternative To Alt1 of Effect 3")]
    public static void AddToAlt1OfEffect3() => AddAlternativeToAlternativeEffect(3, 1);

    private static void AddAlternativeToAlternativeEffect(int mainEffectIndex, int altEffectIndex)
    {
        var card = Selection.activeObject as CardData;
        if (card == null || card.effects.Count == 0)
        {
            Debug.LogError("Select a CardData asset that has at least one effect.");
            return;
        }

        if (mainEffectIndex >= card.effects.Count)
        {
            Debug.LogError($"Card '{card.cardName}' only has {card.effects.Count} effect(s).");
            return;
        }

        var mainEffect = card.effects[mainEffectIndex];

        if (mainEffect.alternativeEffects == null || mainEffect.alternativeEffects.Count == 0)
        {
            Debug.LogError($"Effect #{mainEffectIndex} has no alternative effects. Add one first.");
            return;
        }

        if (altEffectIndex >= mainEffect.alternativeEffects.Count)
        {
            Debug.LogError($"Effect #{mainEffectIndex} only has {mainEffect.alternativeEffects.Count} alternative effect(s).");
            return;
        }

        var altEffect = mainEffect.alternativeEffects[altEffectIndex];

        // Add nested alternative effect
        altEffect.alternativeEffects ??= new();
        altEffect.alternativeEffects.Add(new Effect
        {
            effectName = "NestedAlternative",
            effectTiming = new EffectTiming { effectTimingType = EffectTimingType.Immediate }
        });

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
        Debug.Log($"Added nested alternative effect to Alt{altEffectIndex} of Effect #{mainEffectIndex} in '{card.cardName}'");
    }
}
#endif
