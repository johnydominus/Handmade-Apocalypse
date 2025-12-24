#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class AlternativeEffectNotCounteredAdder
{
    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 0 of Effect 0")]
    public static void AddToAlt0OfEffect0() => AddNotCounteredToAlternative(0, 0);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 0 of Effect 1")]
    public static void AddToAlt0OfEffect1() => AddNotCounteredToAlternative(1, 0);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 0 of Effect 2")]
    public static void AddToAlt0OfEffect2() => AddNotCounteredToAlternative(2, 0);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 0 of Effect 3")]
    public static void AddToAlt0OfEffect3() => AddNotCounteredToAlternative(3, 0);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 0 of Effect 4")]
    public static void AddToAlt0OfEffect4() => AddNotCounteredToAlternative(4, 0);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 1 of Effect 0")]
    public static void AddToAlt1OfEffect0() => AddNotCounteredToAlternative(0, 1);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 1 of Effect 1")]
    public static void AddToAlt1OfEffect1() => AddNotCounteredToAlternative(1, 1);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 1 of Effect 2")]
    public static void AddToAlt1OfEffect2() => AddNotCounteredToAlternative(2, 1);

    [MenuItem("Tools/Cards/Add Not-Countered To Alternative Effect 1 of Effect 3")]
    public static void AddToAlt1OfEffect3() => AddNotCounteredToAlternative(3, 1);

    private static void AddNotCounteredToAlternative(int effectIndex, int alternativeIndex)
    {
        var card = Selection.activeObject as CardData;
        if (card == null || card.effects.Count == 0)
        {
            Debug.LogError("Select a CardData asset that has at least one effect.");
            return;
        }

        if (effectIndex >= card.effects.Count)
        {
            Debug.LogError($"Card '{card.cardName}' only has {card.effects.Count} effect(s).");
            return;
        }

        var mainEffect = card.effects[effectIndex];

        if (mainEffect.alternativeEffects == null || mainEffect.alternativeEffects.Count == 0)
        {
            Debug.LogError($"Effect #{effectIndex} has no alternative effects. Add one first using the Alternative Effect Adder.");
            return;
        }

        if (alternativeIndex >= mainEffect.alternativeEffects.Count)
        {
            Debug.LogError($"Effect #{effectIndex} only has {mainEffect.alternativeEffects.Count} alternative effect(s).");
            return;
        }

        var altEffect = mainEffect.alternativeEffects[alternativeIndex];

        if (!altEffect.isCounterable)
        {
            Debug.LogWarning($"Alternative effect #{alternativeIndex} of Effect #{effectIndex} is not marked as counterable. Set 'Is Counterable' to true first on that alternative effect.");
            return;
        }

        altEffect.effectsIsNotCountered ??= new();
        altEffect.effectsIsNotCountered.Add(new Effect
        {
            effectName = "IfNotCountered",
            effectTiming = new EffectTiming { effectTimingType = EffectTimingType.Immediate }
        });

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
        Debug.Log($"Added not-countered effect to alternative effect #{alternativeIndex} of '{card.cardName}' effect #{effectIndex}");
    }
}
#endif
