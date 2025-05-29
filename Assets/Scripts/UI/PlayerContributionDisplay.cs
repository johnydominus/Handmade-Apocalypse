using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContributionDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI contributionText;
    [SerializeField] private Image backgroundImage;

    public void Setup(string playerName, int contribution, bool isCurrentPlayer)
    {
        playerNameText.text = playerName;
        contributionText.text = $"{contribution} tokens";

        // Highlight current player
        Color bgColor = isCurrentPlayer ? new Color(0.8f, 0.9f, 1f) : Color.white;
        backgroundImage.color = bgColor;
    }
}
