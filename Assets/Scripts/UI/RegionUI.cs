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
        int max = Mathf.Min(currentPlayer.emergencies.Count, soECounteractionUIs.Length);

        for (int i = 0; i < max; i++)
        {
            var emergency = currentPlayer.emergencies[i];

            if (emergency.stateOfEmergency.isActive)
            {
                soECounteractionUIs[i].gameObject.SetActive(true);
                soECounteractionUIs[i].Activate(new SoEContext (emergency.emergencyType, currentPlayer));
            }
            else
            {
                soECounteractionUIs[i].gameObject.SetActive(false);
            }
        }
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
        int max = Mathf.Min(
            emergencyBars.Length,
            emergencyCounters.Length,
            emergencyLabels.Length,
            soECounteractionUIs.Length);

        for (int i = 0; i < max; i++)
        {
            bool active = i < count;
            emergencyBars[i].gameObject.SetActive(active);
            emergencyCounters[i].gameObject.SetActive(active);
            emergencyLabels[i].gameObject.SetActive(active);
            soECounteractionUIs[i].gameObject.SetActive(active);
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
        Debug.Log("Updating emergency bars");
        Debug.Log($"There are {currentPlayer.emergencies.Count} emergencies for {currentPlayer.playerName}");

        int limit = Mathf.Min(currentPlayer.emergencies.Count,
                            emergencyBars.Length,
                            emergencyCounters.Length,
                            emergencyLabels.Length);
        for (int i = 0; i < limit; i++)
        {
            var emergency = currentPlayer.emergencies[i];
            emergencyLabels[i].text = emergency.emergencyType.ToString();
            emergencyBars[i].fillAmount = emergency.emergencyLevel / 10f;
            emergencyCounters[i].text = emergency.stateOfEmergency.isActive
                ? $"!!! {emergency.emergencyType} !!!"
                : $"{emergency.emergencyLevel}";
            emergencyCounters[i].color = emergency.stateOfEmergency.isActive ? Color.red : Color.black;
            Debug.Log($"Emergency {emergency.emergencyType} updated with level {emergency.emergencyLevel}");
        }
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
