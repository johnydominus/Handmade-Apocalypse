#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class NotCounteredEffectAdder
{
    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 0")]
    public static void AddNotCounteredToFirstEffect()
    {
        AddNotCounteredEffectToCardEffect(0);
    }

    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 1")]
    public static void AddNotCounteredToSecondEffect()
    {
        AddNotCounteredEffectToCardEffect(1);
    }

    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 2")]
    public static void AddNotCounteredToThirdEffect()
    {
        AddNotCounteredEffectToCardEffect(2);
    }

    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 3")]
    public static void AddNotCounteredToFourthEffect()
    {
        AddNotCounteredEffectToCardEffect(3);
    }

    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 4")]
    public static void AddNotCounteredToFifthEffect()
    {
        AddNotCounteredEffectToCardEffect(4);
    }

    [MenuItem("Tools/Cards/Add Not-Countered Effect To Selected Card Effect 5")]
    public static void AddNotCounteredToSixthEffect()
    {
        AddNotCounteredEffectToCardEffect(5);
    }

    private static void AddNotCounteredEffectToCardEffect(int effectIndex)
    {
        var card = Selection.activeObject as CardData;
        if (card == null || card.effects.Count == 0)
        {
            Debug.LogError("Select a CardData asset that has at least one effect.");
            return;
        }

        if (effectIndex >= card.effects.Count)
        {
            Debug.LogError($"Card '{card.cardName}' only has {card.effects.Count} effect(s). Cannot add not-countered effect to effect #{effectIndex}.");
            return;
        }

        var eff = card.effects[effectIndex];

        if (!eff.isCounterable)
        {
            Debug.LogWarning($"Effect #{effectIndex} is not marked as counterable. Set 'Is Counterable' to true first.");
            return;
        }

        eff.effectsIsNotCountered ??= new();
        eff.effectsIsNotCountered.Add(new Effect
        {
            effectName = "IfNotCountered",
            effectTiming = new EffectTiming { effectTimingType = EffectTimingType.Immediate }
        });

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
        Debug.Log($"Added not-countered effect to '{card.cardName}' effect #{effectIndex}");
    }
}
#endif
