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
    [Header("UI Prefabs")]
    public GameObject messagePanelPrefab;
    public GameObject effectsPanelPrefab;

    private static readonly List<ThreatType> prototypeThreats = new()
    {
        ThreatType.Pandemic,
        ThreatType.NuclearWar,
        ThreatType.Asteroid
    };

    private List<PlayerController> players;
    private EffectsPanel effectsPanelInstance;
    public GameServices gameServices;
    public BuildType buildType = BuildType.BasicPrototype;
    
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

        gameServices.threatManager = new();
        // Pick threats based on build type
        List<ThreatType> effectiveThreats = buildType switch
        {
            BuildType.BasicPrototype    => prototypeThreats.ToList(),
            BuildType.AdvancedPrototype => selectedThreats.ToList(),
            BuildType.FullGame          => selectedThreats.ToList(),
            _                           => selectedThreats.ToList()
        };

        var sphereNames = EmergencyMapping.GetSphereTypesByThreat(effectiveThreats).ToList();

        foreach (var player in players)
            player.Initialize(sphereNames, players);
        
        MessagePanel.prefab = messagePanelPrefab;
        AsteroidInvestmentManager asteroidManager = AsteroidInvestmentManager.Instance;
        asteroidManager.investmentCost = 50;

        Debug.Log($"GameManager starts the Big Initialization!");
        foreach (var sphereName in sphereNames)
            Debug.Log($"There is {sphereName} in sphereNames!");

        foreach (var player in players)
            player.Initialize(sphereNames, players);

        GameServices.Initialize(gameServices);

        gameServices.threatManager.Initialize(effectiveThreats, buildType);

        gameServices.turnManager = new();
        gameServices.turnManager.Initialize(players, Instantiate(messagePanelPrefab), cardLibrary, buildType);

        gameServices.cardSystem = new();
        gameServices.cardSystem.Initialize(cardLibrary);

        gameServices.soeManager = new();

        gameServices.investmentManager = new();
        gameServices.effectManager = new();
        gameServices.commandManager = new();
        gameServices.delayedCounteractionManager = new();

        if (devTools != null)
        {
            devTools.Initialize(players);
            // Set initial target player
            if (players.Count > 0)
                devTools.SetTargetPlayer(players[0]);
        }

        if (effectsPanelPrefab != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                GameObject effectsPanelObject = Instantiate(effectsPanelPrefab, canvas.transform);
                effectsPanelObject.name = "EffectsPanel";

                // Get and store the EffectsPanel component
                effectsPanelInstance = effectsPanelObject.GetComponent<EffectsPanel>();

                if (effectsPanelInstance != null)
                {
                    Debug.Log("EffectsPanel instantiated and component found successfully");
                }
                else
                {
                    Debug.LogError("EffectsPanel GameObject created but EffectsPanel component not found!");

                    // Debug what components are on the GameObject
                    Component[] components = effectsPanelObject.GetComponents<Component>();
                    Debug.Log($"Components on EffectsPanel GameObject: {components.Length}");
                    foreach (var comp in components)
                    {
                        Debug.Log($" - {comp.GetType().Name}");
                    }
                }
            }
            else
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
        else
        {
            Debug.LogWarning("EffectsPanelPrefab is not assigned in the GameManager.");
        }

        GameEvents.OnGameInitialized.Raise();
        Debug.Log("===== !!! THE GAME INITIALIZED !!!=====");
    }

    private void Start()
    {
        Debug.Log("===== !!! STARTING THE GAME !!! =====");
        GameEvents.OnTurnStarted.Raise(new TurnContext(1, gameServices.turnManager.GetCurrentPlayer()));
        GameServices.Instance.commandManager.ExecuteCommand(new StartTurnCommand(gameServices.turnManager));
    }
    public void OnCurrentEffectsButtonClicked()
    {
        // First try the stored reference
        if (effectsPanelInstance != null)
        {
            effectsPanelInstance.OpenPanel();
            return;
        }

        // Fallback to searching (with debug info)
        Debug.Log("Stored reference is null, searching for EffectsPanel...");

        EffectsPanel effectsPanel = FindFirstObjectByType<EffectsPanel>();
        if (effectsPanel != null)
        {
            effectsPanel.OpenPanel();
        }
        else
        {
            Debug.LogWarning("EffectsPanel not found in scene!");

            // Check if the GameObject exists
            GameObject effectsPanelGO = GameObject.Find("EffectsPanel");
            if (effectsPanelGO != null)
            {
                Debug.Log("EffectsPanel GameObject found, but component missing!");
                Component[] components = effectsPanelGO.GetComponents<Component>();
                Debug.Log($"Components on EffectsPanel GameObject: {components.Length}");
                foreach (var comp in components)
                {
                    Debug.Log($"- {comp.GetType().Name}");
                }
            }
            else
            {
                Debug.LogWarning("EffectsPanel GameObject not found in scene!");
            }
        }
    }
}
