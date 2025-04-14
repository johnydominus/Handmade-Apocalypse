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
    [SerializeField] private TextMeshProUGUI turnHeaderText;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private TokenUI tokenUI;
    [SerializeField] private RegionUI regionUI;
//    [SerializeField] private DevTools devTools;

    private void Awake()
    {
        players = new List<PlayerController>(FindObjectsByType<PlayerController>(FindObjectsSortMode.InstanceID));
        foreach (var player in players)
        {
            player.Initialize();
        }

        GameServices.Initialize(gameServices);


        gameServices.threatManager = new ThreatManager();
        gameServices.threatManager.Initialize();

        gameServices.turnManager = new TurnManager();
        gameServices.turnManager.Initialize(players);
        gameServices.turnManager.turnHeaderText = turnHeaderText;
        gameServices.turnManager.turnNumberText = turnNumberText;
        gameServices.turnManager.tokenUI = tokenUI;
        gameServices.turnManager.regionUI = regionUI;
//        gameServices.turnManager.devTools = devTools;

        gameServices.cardSystem = new CardSystem();
        gameServices.cardSystem.Initialize(cardLibrary);

        gameServices.commandManager = new CommandManager();

        GameEvents.OnGameInitialized.Raise();
    }
}
