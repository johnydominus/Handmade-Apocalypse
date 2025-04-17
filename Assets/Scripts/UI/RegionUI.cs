using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;

public class RegionUI : MonoBehaviour
{
    public List<InvestmentSphereRelay> spheresUI;
    public List<InvestmentForecastDisplay> forecastUI;

    public TextMeshProUGUI regionHeader;
    public GameObject emergencyPanel;
    public GameObject investmentPanel;

    public Image[] emergencyBars; // 0–10 fillAmount bars
    public TextMeshProUGUI[] emergencyLabels;

    public Button emergenciesTabButton;
    public Button investmentsTabButton;

    public TMP_Text emergenciesTabLabel;
    public TMP_Text investmentsTabLabel;

    private PlayerController currentPlayer;

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        SetRegion(turnContext.player, turnContext.player);
    }
    
    public void SetRegion(PlayerController regionOwner, PlayerController investor)
    {
        currentPlayer = regionOwner;
        regionHeader.text = $"Region: {regionOwner.playerName}";
        UpdateEmergencyBars();

        for (int i = 0; i < spheresUI.Count; i++)
        {
            spheresUI[i].SetContext(regionOwner, investor);
            spheresUI[i].name = regionOwner.investments[i].sphereName;
            spheresUI[i].UpdateAmountText();
            forecastUI[i].UpdateForecast(regionOwner, investor);
        }
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
