using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<PlayerController> players;
    public GameServices gameServices;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private List<ThreatType> selectedThreats;
    //    [SerializeField] private DevTools devTools;

    private void Awake()
    {
        players = new List<PlayerController>(FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID));
        var sphereNames = ThreatToSphereMapper.GetSphereNames(selectedThreats);

        Debug.Log($"GameManager starts the Big Initialization!");
        foreach (var sphereName in sphereNames)
            Debug.Log($"There is {sphereName} in sphereNames!");

        foreach (var player in players)
            player.Initialize(sphereNames, players);

        GameServices.Initialize(gameServices);

        gameServices.threatManager = new();
        gameServices.threatManager.Initialize();

        gameServices.turnManager = new();
        gameServices.turnManager.Initialize(players);

        gameServices.cardSystem = new();
        gameServices.cardSystem.Initialize(cardLibrary);

        gameServices.investmentManager = new();

        gameServices.commandManager = new();
        //        gameServices.turnManager.devTools = devTools;

        GameEvents.OnGameInitialized.Raise();
    }

    private void Start()
    {
        gameServices.turnManager.StartTurn();
    }
}
