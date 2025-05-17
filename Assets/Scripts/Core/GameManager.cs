using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum BuildType
{
    BasicPrototype,
    AdvancedPrototype,
    FullGame
}
public class GameManager : MonoBehaviour
{
    private List<PlayerController> players;
    public GameServices gameServices;
    public BuildType buildType = BuildType.FullGame;
    public GameObject messagePanelPrefab;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private List<ThreatType> selectedThreats;
    [SerializeField] private DevTools devTools;
    [SerializeField] private UIWInLossMessage winLossMessage;

    private void OnEnable()
    {
        GameEvents.OnTurnEnd.RegisterListener(OnTurnEnd);
        GameEvents.OnVictory.RegisterListener(OnVictory);
        GameEvents.OnLoss.RegisterListener(OnLoss);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnEnd.UnregisterListener(OnTurnEnd);
        GameEvents.OnVictory.UnregisterListener(OnVictory);
        GameEvents.OnLoss.UnregisterListener(OnLoss);
    }

    private void OnTurnEnd()
    {
        CheckAllRegionSoEs();
    }

    private void OnVictory()
    {
        winLossMessage.gameObject.SetActive(true);
        winLossMessage.SetMessage(true, null);
    }

    private void OnLoss(ThreatType fatalThreat)
    {
        // TODO: Implement getting the fatal threat

        winLossMessage.gameObject.SetActive(true);
        winLossMessage.SetMessage(false, fatalThreat);
    }

    private void CheckAllRegionSoEs()
    {
        // Get all possible threat types from the mapping
        foreach (var entry in selectedThreats)
        {
            bool allRegionsHaveActiveSoE = true;
            EmergencyType? relevantEmergencyType = EmergencyMapping.GetByThreat(entry).emergency;

            // Check if all regions have this SoE active
            foreach (var player in players)
            {
                var emergency = player.emergencies.FirstOrDefault(e => e.emergencyType == relevantEmergencyType);
                if (emergency == null || !emergency.stateOfEmergency.isActive)
                {
                    allRegionsHaveActiveSoE = false;
                    break;
                }
            }

            if (allRegionsHaveActiveSoE)
            {
                // Increment the counter for this threat
                gameServices.threatManager.threatTracker.IncrementSoETurnCounter(entry);
            }
            else
            {
                // Reset the counter for this threat
                gameServices.threatManager.threatTracker.ResetSoETurnCounter(entry);
            }
        }
    }

    private void Awake()
    {
        players = new List<PlayerController>(FindObjectsByType<PlayerController>(FindObjectsSortMode.None));
        players.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name));

        var sphereNames = EmergencyMapping.GetSphereTypesByThreat(selectedThreats);
        MessagePanel.prefab = messagePanelPrefab;

        Debug.Log($"GameManager starts the Big Initialization!");
        foreach (var sphereName in sphereNames)
            Debug.Log($"There is {sphereName} in sphereNames!");

        foreach (var player in players)
            player.Initialize(sphereNames, players);

        GameServices.Initialize(gameServices);

        gameServices.threatManager = new();
        gameServices.threatManager.Initialize(selectedThreats, buildType);

        gameServices.turnManager = new();
        gameServices.turnManager.Initialize(players, Instantiate(messagePanelPrefab), cardLibrary);

        gameServices.cardSystem = new();
        gameServices.cardSystem.Initialize(cardLibrary);

        gameServices.soeManager = new();

        gameServices.investmentManager = new();
        gameServices.effectManager = new();
        gameServices.commandManager = new();

        GameEvents.OnGameInitialized.Raise();
        Debug.Log("===== !!! THE GAME INITIALIZED !!!=====");
    }

    private void Start()
    {
        Debug.Log("===== !!! STARTING THE GAME !!! =====");
        GameEvents.OnTurnStarted.Raise(new TurnContext(1, gameServices.turnManager.GetCurrentPlayer()));
        GameServices.Instance.commandManager.ExecuteCommand(new StartTurnCommand(gameServices.turnManager));
    }
}
