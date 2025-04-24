using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<PlayerController> players;
    public GameServices gameServices;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private List<ThreatType> selectedThreats;
    [SerializeField] private DevTools devTools;

    private void Awake()
    {
        players = new List<PlayerController>(FindObjectsByType<PlayerController>(FindObjectsSortMode.None));
        players.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name));

        var sphereNames = EmergencyMapping.GetSphereTypesByThreat(selectedThreats);

        Debug.Log($"GameManager starts the Big Initialization!");
        foreach (var sphereName in sphereNames)
            Debug.Log($"There is {sphereName} in sphereNames!");

        foreach (var player in players)
            player.Initialize(sphereNames, players);

        GameServices.Initialize(gameServices);

        gameServices.threatManager = new();
        gameServices.threatManager.Initialize(selectedThreats);

        gameServices.turnManager = new();
        gameServices.turnManager.Initialize(players);

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
        gameServices.turnManager.StartTurn();
    }
}
