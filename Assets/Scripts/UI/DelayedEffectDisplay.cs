using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DelayedEffectDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI turnsRemainingText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Button counteractButton;
    [SerializeField] private Image backgroundImage;

    private DelayedCounterableEffect delayedEffect;

    private void Awake()
    {
        counteractButton.onClick.AddListener(OpenCounteractionPanel);   
    }

    public void Setup(DelayedCounterableEffect effect)
    {
        delayedEffect = effect;

        effectNameText.text = effect.description;

        // Update turns remaining
        if (effect.turnsRemaining > 1)
            turnsRemainingText.text = $"{effect.turnsRemaining} turns remaining";
        else if (effect.turnsRemaining == 1)
            turnsRemainingText.text = $"FINAL TURN!";
        else
            turnsRemainingText.text = "TRIGGERING NOW!";

        // Update progress
        float progress = (float)effect.tokensContributed / effect.tokensRequired;
        progressSlider.value = progress; ;
        progressText.text = $"{effect.tokensContributed} / {effect.tokensRequired} tokens";

        // Color coding based on urgency and progress
        Color bgColor = Color.white;
        if (effect.isCountered)
        {
            bgColor = new Color(0.8f, 1f, 0,8f);    // Light green if countered
            counteractButton.interactable = false;
            counteractButton.GetComponentInChildren<TextMeshProUGUI>().text = "COUNTERED";
        }
        else if (effect.turnsRemaining <= 1)
        {
            bgColor = new Color(1f, 0.6f, 0.6f);    // Red if urgent
            counteractButton.interactable = true;
            counteractButton.GetComponentInChildren<TextMeshProUGUI>().text = "URGENT!";
        }
        else if (progress >= 0.5f)
        {
            bgColor = new Color(1f, 1f, 0.8f);      // Light yellow if making progress
            counteractButton.interactable = true;
            counteractButton.GetComponentInChildren<TextMeshProUGUI>().text = "Contribute";
        }
        else
        {
            bgColor = new Color(1f, 0.9f, 0, 9f);   // Light pink if not much progress
            counteractButton.interactable = true;
            counteractButton.GetComponentInChildren<TextMeshProUGUI>().text = "Contribute";
        }

        backgroundImage.color = bgColor;
    }

    private void OpenCounteractionPanel()
    {
        if (delayedEffect != null && !delayedEffect.isCountered && !delayedEffect.hasTriggered)
        {
            // Find or create the counteraction panel
            DelayedEffectCounteractionPanel panel = FindFirstObjectByType<DelayedEffectCounteractionPanel>();
            if (panel != null)
            {
                panel.OpenPanel(delayedEffect);
            }
            else
            {
                Debug.LogWarning("DelayedEffectCounteractionPanel not found in scene!");
            }
        }
    }
}
