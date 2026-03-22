using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveEffectDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image backgroundImage;

    public void Setup(ActiveEffect effect, string targetName)
    {
        // Clean description from the summary generator
        string summary = CardVisualConfig.GenerateEffectSummary(
            new System.Collections.Generic.List<Effect> { effect.effect });
        effectNameText.text = string.IsNullOrEmpty(summary) ? effect.effect.effectName : summary;
        
        targetText.text = $"Target: {targetName}";

        durationText.text = effect.timing.effectTimingType switch
        {
            EffectTimingType.Immediate      => "Duration: Immediate",
            EffectTimingType.CurrentTurn    => "Duration: Current turn",
            EffectTimingType.MultiTurn      => $"{effect.timing.duration - effect.timing.turnsElapsed} turns left",
            EffectTimingType.Delayed        => $"Triggers in: {effect.timing.delay - effect.timing.turnsElapsed} turns",
            EffectTimingType.Recurring      => $"Recurring: {effect.timing.interval} turn interval",
            _ => "Duration: Unknown"
        };
        
/*       // Show duration info 
        if (effect.timing.effectTimingType == EffectTimingType.Immediate)
        {
            durationText.text = "Duration: Immediate";
        }
        else if (effect.timing.effectTimingType == EffectTimingType.CurrentTurn)
        {
            durationText.text = "Duration: Current turn";
        }
        else if (effect.timing.effectTimingType == EffectTimingType.MultiTurn)
        {
            int remaining = effect.timing.duration - effect.timing.turnsElapsed;
            durationText.text = $"Duration: {remaining} turns left";
        }
        else if (effect.timing.effectTimingType == EffectTimingType.Delayed)
        {
            int remaining = effect.timing.delay - effect.timing.turnsElapsed;
            durationText.text = $"Triggers in: {remaining} turns";
        }
        else if (effect.timing.effectTimingType == EffectTimingType.Recurring)
        {
            durationText.text = $"Recurring: {effect.timing.interval} turn interval";
        }

        // Show effect value/type
        string valueStr = "";
        switch (effect.effect.effectType)
        {
            case EffectType.Add:
                valueStr = effect.effect.value >= 0 ? $"+{effect.effect.value}" : effect.effect.value.ToString();
                break;
            case EffectType.Multiply:
                valueStr = $"x{effect.effect.value}";
                break;
            case EffectType.Block:
                valueStr = $"BLOCKED";
                break;
            default:
                valueStr = effect.effect.value.ToString();
                break;
        }
*/

        if (valueText != null) valueText.gameObject.SetActive(false);

        backgroundImage.color = effect.effect.effectType == EffectType.Block 
            ? new Color(0.9f, 0.9f, 0.9f)
            : effect.effect.value > 0
                ? new Color(0.8f, 1f, 0.8f)
                : new Color(1f, 0.8f, 0.8f);
    }
}
