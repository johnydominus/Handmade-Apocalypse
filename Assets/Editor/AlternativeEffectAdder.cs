#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class AlternativeEffectAdder
{
    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 0")]
    public static void AddAltToFirstEffect()
    {
        AddAlternativeEffectToCardEffect(0);
    }

    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 1")]
    public static void AddAltToSecondEffect()
    {
        AddAlternativeEffectToCardEffect(1);
    }

    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 2")]
    public static void AddAltToThirdEffect()
    {
        AddAlternativeEffectToCardEffect(2);
    }

    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 3")]
    public static void AddAltToFourthEffect()
    {
        AddAlternativeEffectToCardEffect(3);
    }

    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 4")]
    public static void AddAltToFifthEffect()
    {
        AddAlternativeEffectToCardEffect(4);
    }

    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 5")]
    public static void AddAltToSixthEffect()
    {
        AddAlternativeEffectToCardEffect(5);
    }

    private static void AddAlternativeEffectToCardEffect(int effectIndex)
    {
        var card = Selection.activeObject as CardData;
        if (card == null || card.effects.Count == 0)
        {
            Debug.LogError("Select a CardData asset that has at least one effect.");
            return;
        }

        if (effectIndex >= card.effects.Count)
        {
            Debug.LogError($"Card '{card.cardName}' only has {card.effects.Count} effect(s). Cannot add alternative to effect #{effectIndex}.");
            return;
        }

        var eff = card.effects[effectIndex];
        eff.alternativeEffects ??= new();
        eff.alternativeEffects.Add(new Effect
        {
            effectName = "Alternative",
            effectTiming = new EffectTiming { effectTimingType = EffectTimingType.Immediate }
        });

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
        Debug.Log($"Added alternative effect to '{card.cardName}' effect #{effectIndex}");
    }
}
#endif
