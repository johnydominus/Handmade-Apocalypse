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
        effectNameText.text = effect.effect.effectName;
        targetText.text = $"Target: {targetName}";

        // Show duration info 
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
        valueText.text = $"Effect: {valueStr}";

        // Color coding based on effect type
        Color bgColor = Color.white;
        if (effect.effect.value > 0 && effect.effect.effectType == EffectType.Add)
        {
            bgColor = new Color(0.8f, 1f, 0.8f);    // Light green for positive
        }
        else if (effect.effect.value < 0 && effect.effect.effectType == EffectType.Add)
        {
            bgColor = new Color(1f, 0.8f, 0.8f);    // Light red for negative
        }
        else if (effect.effect.effectType == EffectType.Block)
        {
            bgColor = new Color(0.9f, 0.9f, 0.9f);    // Light gray for block
        }

        backgroundImage.color = bgColor;
    }
}
