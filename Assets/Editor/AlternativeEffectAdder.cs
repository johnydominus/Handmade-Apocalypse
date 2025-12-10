#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class AlternativeEffectAdder
{
    [MenuItem("Tools/Cards/Add Alternative Effect To Selected Card Effect 0")]
    public static void AddAltToFirstEffect()
    {
        var card = Selection.activeObject as CardData;
        if (card == null || card.effects.Count == 0)
        {
            Debug.LogError("Select a CardData asset that has at least one effect.");
            return;
        }

        var eff = card.effects[0];
        eff.alternativeEffects ??= new();
        eff.alternativeEffects.Add(new Effect
        {
            effectName = "Alternative",
            effectTiming = new EffectTiming { effectTimingType = EffectTimingType.Immediate }
        });

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
        Debug.Log($"Added alternative effect to '{card.cardName}' effect #0");
    }
}
#endif
