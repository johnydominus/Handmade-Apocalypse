using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegionUI : MonoBehaviour
{
    public TextMeshProUGUI regionHeader;
    public GameObject emergencyPanel;
    public GameObject investmentPanel;

    public InvestmentForecastDisplay astroForecast;
    public InvestmentForecastDisplay diploForecast;
    public InvestmentForecastDisplay medForecast;

    public InvestmentButtonRelay astroButtonRelay;
    public InvestmentButtonRelay diploButtonRelay;
    public InvestmentButtonRelay medButtonRelay;

    public Image[] emergencyBars; // 0–10 fillAmount bars
    public TextMeshProUGUI[] emergencyLabels;

    public Button emergenciesTabButton;
    public Button investmentsTabButton;

    public TMP_Text emergenciesTabLabel;
    public TMP_Text investmentsTabLabel;

    private PlayerController currentPlayer;

    public void SetRegion(PlayerController regionOwner, PlayerController investor)
    {
        currentPlayer = regionOwner;
        regionHeader.text = $"Region: {regionOwner.playerName}";
        UpdateEmergencyBars();

        // Update forecasts
//        astroForecast.UpdateForecast(regionOwner, investor);
//        diploForecast.UpdateForecast(regionOwner, investor);
//        medForecast.UpdateForecast(regionOwner, investor);

        // Update token investment UI
        astroButtonRelay.SetContext(regionOwner, investor);
        diploButtonRelay.SetContext(regionOwner, investor);
        medButtonRelay.SetContext(regionOwner, investor);

        astroButtonRelay.UpdateAmountText();
        diploButtonRelay.UpdateAmountText();
        medButtonRelay.UpdateAmountText();
    }

    public void SwitchTab(string tab)
    {
        bool isEmergency = tab == "Emergencies";

        emergencyPanel.SetActive(isEmergency);
        investmentPanel.SetActive(!isEmergency);

        // Style the tab labels
        emergenciesTabLabel.fontStyle = isEmergency ? FontStyles.Bold : FontStyles.Normal;
        investmentsTabLabel.fontStyle = isEmergency ? FontStyles.Normal : FontStyles.Bold;

        emergenciesTabLabel.fontSize = isEmergency ? 16 : 15;
        investmentsTabLabel.fontSize = isEmergency ? 15 : 17;
    }

    public void UpdateEmergencyBars()
    {
        for (int i = 0; i < emergencyBars.Length; i++)
        {
            float level = currentPlayer.emergencyLevels[i];
            emergencyBars[i].fillAmount = level / 10f;
            emergencyLabels[i].text = $"{level}/10";

            emergencyLabels[i].color = currentPlayer.isEmergencyActive[i]
                ? Color.red
                : Color.black;
        }
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
