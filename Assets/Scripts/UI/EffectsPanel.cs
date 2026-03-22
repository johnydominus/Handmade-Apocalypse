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
    foreach (Transform child in activeEffectsContainer)
        Destroy(child.gameObject);

    var ongoing = GameServices.Instance.effectManager.GetActiveEffects();
    var thisTurn = GameServices.Instance.effectManager.GetCurrentTurnEffects()
        .Where(e => !ongoing.Contains(e))   // avoid duplicates
        .ToList();

    bool hasAnything = ongoing.Count > 0 || thisTurn.Count > 0;
    noActiveEffectsText.gameObject.SetActive(!hasAnything);
    if (!hasAnything) return;

    foreach (var effect in ongoing)
        CreateActiveEffectDisplay(effect, effect.player?.playerName ?? "Global");

    foreach (var effect in thisTurn)
        CreateActiveEffectDisplay(effect, effect.player?.playerName ?? "Global");
}

private void RefreshDelayedEffects()
{
    foreach (Transform child in delayedEffectsContainer)
        Destroy(child.gameObject);

    var counterable = GameServices.Instance.delayedCounteractionManager.GetActiveDelayedEffects();
    var pending = GameServices.Instance.effectManager.GetPendingDelayedEffects();

    bool hasAnything = counterable.Count > 0 || pending.Count > 0;
    noDelayedEffectsText.gameObject.SetActive(!hasAnything);
    if (!hasAnything) return;

    // Counterable delayed effects (existing system)
    foreach (var effect in counterable)
        CreateDelayedEffectDisplay(effect);

    // Non-counterable delayed effects from effectManager
    foreach (var effect in pending)
        CreateActiveEffectDisplay(effect, effect.player?.playerName ?? "Global");
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
