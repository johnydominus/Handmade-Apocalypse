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
    public BuildType buildType = BuildType.BasicPrototype;
    public GameObject messagePanelPrefab;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private List<ThreatType> selectedThreats;
    [SerializeField] private DevTools devTools;

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
    }

    private void Start()
    {
        Debug.Log("Starting the game!");
        GameEvents.OnTurnStarted.Raise(new TurnContext(1, gameServices.turnManager.GetCurrentPlayer()));
        GameServices.Instance.commandManager.ExecuteCommand(new StartTurnCommand(gameServices.turnManager));
    }
}
