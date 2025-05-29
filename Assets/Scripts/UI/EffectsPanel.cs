using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectsPanel : MonoBehaviour
{
    [Header ("Panel References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;

    [Header("Active Effects Section")]
    [SerializeField] private Transform activeEffectsContainer;
    [SerializeField] private GameObject activeEffectsPrefab;
    [SerializeField] private TextMeshProUGUI noActiveEffectsText;

    [Header("Delayed Effects Section")]
    [SerializeField] private Transform delayedEffectsContainer;
    [SerializeField] private GameObject delayedEffectsPrefab;
    [SerializeField] private TextMeshProUGUI noDelayedEffectsText;

    private void Awake()
    {
        // Hide panel initially
        panelRoot.SetActive(false);

        // Set up button listeners
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(ClosePanel);

        Debug.Log("EffectsPanel intialized.");
    }

    private void OnEnable()
    {
        // Subscribe to effect events
        GameEvents.OnDelayedEffectRegistered.RegisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectUpdated.RegisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectCountered.RegisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectTriggered.RegisterListener(OnDelayedEffectChanged);
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        GameEvents.OnDelayedEffectRegistered.UnregisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectUpdated.UnregisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectCountered.UnregisterListener(OnDelayedEffectChanged);
        GameEvents.OnDelayedEffectTriggered.UnregisterListener(OnDelayedEffectChanged);
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    public void OpenPanel()
    {
        panelRoot.SetActive(true);
        RefreshEffectsDisplay();
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
    }

    private void OnTurnStarted(TurnContext context)
    {
        // Refresh display when panel is open
        if (panelRoot.activeSelf)
            RefreshEffectsDisplay();
    }

    private void OnDelayedEffectChanged(DelayedCounterableEffect effect)
    {
        // Refresh display when delayed effect is changed
        if (panelRoot.activeSelf)
        {
            RefreshEffectsDisplay();
        }
    }

    private void RefreshEffectsDisplay()
    {
        RefreshActiveEffects();
        RefreshDelayedEffects();
    }

    private void RefreshActiveEffects()
    {
        // Clear existing displays
        foreach (Transform child in activeEffectsContainer)
        {
            Destroy(child.gameObject);
        }

        // Get all active effects
        var activeEffects = GameServices.Instance.effectManager.GetActiveEffects();

        if (activeEffects.Count == 0)
        {
            noActiveEffectsText.gameObject.SetActive(true);
            return;
        }

        noActiveEffectsText.gameObject.SetActive(false);

        // Group effects by player for better organization
        var globalEffects = activeEffects.Where(e => e.player == null).ToList();
        var playerEffects = activeEffects.Where(e => e.player != null)
                                        .GroupBy(e => e.player.playerName)
                                        .ToList();

        // Display global effects first
        foreach (var effect in globalEffects)
        {
            CreateActiveEffectDisplay(effect, "Global");
        }

        // Display player-specific effects
        foreach (var playerGroup in playerEffects)
        {
            foreach (var effect in playerGroup)
            {
                CreateActiveEffectDisplay(effect, playerGroup.Key);
            }
        }
    }

    private void RefreshDelayedEffects()
    {
        // Clear existing displays
        foreach (Transform child in delayedEffectsContainer)
        {
            Destroy(child.gameObject);
        }

        // Get all active delayed effects
        var delayedEffects = GameServices.Instance.delayedCounteractionManager.GetActiveDelayedEffects();

        if (delayedEffects.Count == 0)
        {
            noDelayedEffectsText.gameObject.SetActive(true);
            return;
        }

        noDelayedEffectsText.gameObject.SetActive(false);

        // Display each delayed effect
        foreach (var delayedEffect in delayedEffects)
        {
            CreateDelayedEffectDisplay(delayedEffect);
        }
    }

    private void CreateActiveEffectDisplay(ActiveEffect effect, string targetName)
    {
        GameObject effectObj = Instantiate(activeEffectsPrefab, activeEffectsContainer);
        ActiveEffectDisplay display = effectObj.GetComponent<ActiveEffectDisplay>();

        if (display != null)
        {
            display.Setup(effect, targetName);
        }
    }

    private void CreateDelayedEffectDisplay(DelayedCounterableEffect delayedEffect)
    {
        GameObject effectObj = Instantiate(delayedEffectsPrefab, delayedEffectsContainer);
        DelayedEffectDisplay display = effectObj.GetComponent<DelayedEffectDisplay>();

        if (display != null) 
        {
            display.Setup(delayedEffect);
        }
    }
}
