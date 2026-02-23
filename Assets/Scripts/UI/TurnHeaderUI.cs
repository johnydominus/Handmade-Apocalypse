using System.Collections;
using TMPro;
using UnityEngine;

public class TurnHeaderUI : MonoBehaviour
{
    public TextMeshProUGUI playersName;
    public TextMeshProUGUI turnNumber;

    private int lastTurnNumber = -1;
    private bool hasInitialized = false;

    [Header("Animation Colors")]
    public Color popInColor = new Color(1.0f, 0.85f, 0.3f);
    public Color punchColor = new Color(1f, 0.5f, 0f);

    private Color defaultColor;

    private void Awake()
    {
        defaultColor = playersName.color;
    }

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        Debug.Log($"TurnHeaderUI: OnTurnStarted: {turnContext.player.playerName}'s turn");

        playersName.text = $"{turnContext.player.playerName}'s turn";

        if (!hasInitialized)
        {
            hasInitialized = true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(PopIn(playersName));   
        }

        if (turnContext.turnNumber != lastTurnNumber)
        {
            turnNumber.text = turnContext.turnNumber.ToString();
            if (hasInitialized)
                StartCoroutine(Punch(turnNumber));
            lastTurnNumber = turnContext.turnNumber;
        }
    }

    private IEnumerator PopIn(TextMeshProUGUI label)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            // Overshoot then settle
            float scale = progress < 0.5f
                ? Mathf.Lerp(0f, 1.5f, progress / 0.55f)
                : Mathf.Lerp(1.5f, 1.0f, (progress - 0.55f) / 0.45f);
            label.transform.localScale = Vector3.one * scale;
            label.color = Color.Lerp(popInColor, defaultColor, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }
        label.transform.localScale = Vector3.one;
        label.color = defaultColor;
    }

    private IEnumerator Punch(TextMeshProUGUI label)
    {
        float duration = 0.6f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float scale = 1f + 0.4f * Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress); // Punch effect
            label.transform.localScale = Vector3.one * scale;
            label.color = Color.Lerp(punchColor, defaultColor, progress);
            elapsed += Time.deltaTime;
            yield return null;
        label.transform.localScale = Vector3.one;
        label.color = defaultColor;
        }
    }
}
