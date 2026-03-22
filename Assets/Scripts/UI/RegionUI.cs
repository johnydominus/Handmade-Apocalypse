using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;

public class RegionUI : MonoBehaviour
{
    [SerializeField] private SoECounteractionUI[] soECounteractionUIs; // Array matching your emergency slots

    public List<InvestmentSphereRelay> spheresUI;
    public List<InvestmentForecastDisplay> forecastUI;

    public TextMeshProUGUI regionHeader;
    public GameObject emergencyPanel;
    public GameObject investmentPanel;

    public Image[] emergencyBars; // 0�10 fillAmount bars
    public TextMeshProUGUI[] emergencyCounters;
    public TextMeshProUGUI[] emergencyLabels;

    public Button emergenciesTabButton;
    public Button investmentsTabButton;

    public TMP_Text emergenciesTabLabel;
    public TMP_Text investmentsTabLabel;

    private PlayerController currentPlayer;

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
        GameEvents.OnSoEActivated.RegisterListener(OnSoEActivated);
        GameEvents.OnSoEDeactivated.RegisterListener(OnSoEDeactivated);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
        GameEvents.OnSoEActivated.UnregisterListener(OnSoEActivated);
        GameEvents.OnSoEDeactivated.UnregisterListener(OnSoEDeactivated);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        SetRegion(turnContext.player, turnContext.player);
    }
    
    private void OnSoEActivated(SoEContext context)
    {
        if (currentPlayer == null) return;
        UpdateEmergencyBars();
    }

    private void OnSoEDeactivated(SoEContext context)
    {
        UpdateEmergencyBars();
    }

    public void SetRegion(PlayerController regionOwner, PlayerController investor)
    {
        currentPlayer = regionOwner;
        regionHeader.text = $"Region: {regionOwner.playerName}";

        int count = currentPlayer.emergencies.Count;

        for (int i = 0; i < count; i++)
        {
            if (i < emergencyBars.Length) emergencyBars[i].gameObject.SetActive(true);
            if (i < emergencyCounters.Length) emergencyCounters[i].gameObject.SetActive(true);
            if (i < emergencyLabels.Length) emergencyLabels[i].gameObject.SetActive(true);
            if (i < soECounteractionUIs.Length) soECounteractionUIs[i].gameObject.SetActive(false);
        }

        UpdateEmergencyBars();

        for (int i = 0; i < spheresUI.Count; i++)
        {
            spheresUI[i].SetContext(regionOwner, investor);
            spheresUI[i].name = regionOwner.investments[i].sphereName.ToString();
            spheresUI[i].UpdateAmountText();
            forecastUI[i].UpdateForecast(regionOwner, investor);
        }
    }

    public void SwitchTab(string tab)
    {
        bool isEmergency = tab == "Emergencies";

        emergencyPanel.SetActive(isEmergency);
        investmentPanel.SetActive(!isEmergency);

        foreach (var relay in spheresUI)
            relay.Setup();

        // Style the tab labels
        emergenciesTabLabel.fontStyle = isEmergency ? FontStyles.Bold : FontStyles.Normal;
        investmentsTabLabel.fontStyle = isEmergency ? FontStyles.Normal : FontStyles.Bold;

        emergenciesTabLabel.fontSize = isEmergency ? 16 : 15;
        investmentsTabLabel.fontSize = isEmergency ? 15 : 17;
    }

    public void UpdateEmergencyBars()
    {
        if (currentPlayer == null) return;

        for (int i = 0; i < currentPlayer.emergencies.Count; i++)
        {
            var emergency = currentPlayer.emergencies[i];
            bool isActive = emergency.stateOfEmergency.isActive;

            if (i < emergencyLabels.Length) emergencyLabels[i].text = emergency.emergencyType.ToString();
            if (i < emergencyBars.Length) emergencyBars[i].fillAmount = emergency.emergencyLevel / 10f;
            if (i < emergencyCounters.Length)
            {            emergencyCounters[i].text = isActive
                ? $"!!! {emergency.emergencyType} !!!"
                : $"{emergency.emergencyLevel}";
            emergencyCounters[i].color = isActive ? Color.red : Color.black;
            }
            if (i < soECounteractionUIs.Length)
            {
                soECounteractionUIs[i].gameObject.SetActive(isActive);
                if (isActive)
                    soECounteractionUIs[i].Activate(new SoEContext(emergency.emergencyType, currentPlayer));
            }
        }
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
