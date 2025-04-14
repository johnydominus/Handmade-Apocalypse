using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ThreatDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetValue(int value)
    {
        if (slider != null)
            slider.value = value / 100f;

        if (valueText != null)
            valueText.text = value.ToString();
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