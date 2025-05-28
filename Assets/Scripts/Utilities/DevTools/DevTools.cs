using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using UnityEngine.UI;

public class DevTools : MonoBehaviour
{
    public GameObject devPanel;
    public SoEManager soeManager;
    public TextMeshProUGUI[] emergencyLabels;
    public DevEmergencyControl[] emergencyControls;
    public RegionUI regionPanel;

    public PlayerController targetPlayer { get; private set; }
    public TextMeshProUGUI playerNameLabel; // ← assign in Inspector

    List<PlayerController> players;
    private int currentIndex = 0;

    public void Initialize(List<PlayerController> thePlayers)
    {
        players = thePlayers;
    }

    public void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    public void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        SetTargetPlayer(turnContext.player);
    }

    public void SetTargetPlayer(PlayerController player)
    {
        if (player == null) return;

        targetPlayer = player;

        if (playerNameLabel != null)
            playerNameLabel.text = player.playerName;
        else
            Debug.LogWarning("Player name label not assigned in the Inspector!");

        Debug.Log($"DevTools: Switched to {player.playerName}");
    }

    public void NextPlayer()
    {
        currentIndex = (currentIndex + 1) % players.Count;
        SetTargetPlayer(players[currentIndex]);
    }

    public void PreviousPlayer()
    {
        currentIndex = (currentIndex - 1 + players.Count) % players.Count;
        SetTargetPlayer(players[currentIndex]);
    }

    public void TogglePanel()
    {
        devPanel.SetActive(!devPanel.activeSelf);
    }

    public void IncreaseEmergency(EmergencyType? emergencyType)
    {
        GameServices.Instance.soeManager.IncreaseEmergency(targetPlayer, emergencyType, 1);
        regionPanel.UpdateEmergencyBars();
    }

    public void DecreaseEmergency(EmergencyType? emergencyType)
    {
        GameServices.Instance.soeManager.DecreaseEmergency(targetPlayer, emergencyType, 1);
        regionPanel.UpdateEmergencyBars();
    }

    public void OpenPanel()
    {
        devPanel.SetActive(true);
        
        int i = 0;

        foreach (var emergency in targetPlayer.emergencies)
        {
            emergencyLabels[i].text = emergency.emergencyType.ToString();
            emergencyControls[i].Initialize(emergency.emergencyType, this);
            i++;
        }
    }

    public void ClosePanel()
    {
        devPanel.SetActive(false);
    }
}