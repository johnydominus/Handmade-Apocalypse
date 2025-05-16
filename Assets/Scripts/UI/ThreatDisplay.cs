using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class ThreatDisplay : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    [SerializeField] private RectTransform backgroundRect;  // Reference to the RectTransform of the background
    [SerializeField] private RectTransform sliderRect;      // Reference to the RectTransform of the slider

    private void Awake()
    {
        // Ensure slider is configured correctly
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.5f; // Initialize to 0
        }
    }

    public void SetValue(int value, int delta = 0)
    {
        if (slider != null)
        {
            // Preserve the original size of the slider and background
            Vector2 originalSize = sliderRect.sizeDelta;
            Vector2 backgroundSize = backgroundRect != null ? backgroundRect.sizeDelta : Vector2.zero;

            // Set the value
            slider.value = value / 100f;

            // Ensure the slider and the background maintain their size
            if (slider != null)
                sliderRect.sizeDelta = originalSize;

            if (backgroundRect != null)
                backgroundRect.sizeDelta = backgroundSize;
        }   

        if (valueText != null) {
            if (delta != 0)
                valueText.text = $"{value} ({(delta > 0 ? "+" : "")}{delta})";
            else
                valueText.text = value.ToString();
        }
    }
}

/* OLD IMPLEMENTATION
public class ThreatUI : MonoBehaviour
{
    public TextMeshProUGUI threatNameText;
    public TextMeshProUGUI percentText;
    public Image fillBar;

    public void UpdateDisplay(Threat threat)
    {
        string deltaPart = threat.threatDelta != 0
            ? $" ({(threat.threatDelta > 0 ? "+" : "")}{threat.threatDelta})"
            : "";

        percentText.text = $"{threat.threatValue}%{deltaPart}";
        threatNameText.text = threat.threatName;
        fillBar.fillAmount = threat.threatValue / 100f;
    }
}
*/