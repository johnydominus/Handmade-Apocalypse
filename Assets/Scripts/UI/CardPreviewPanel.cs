using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardPreviewPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI shortDescText;
    [SerializeField] private TextMeshProUGUI fullDescText;
    [SerializeField] private Image background;
    [SerializeField] private Image sphereIconImage;
    [SerializeField] private float borderThickness = 6f;

    private static CardPreviewPanel _instance;
    public static CardPreviewPanel Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<CardPreviewPanel>(FindObjectsInactive.Include);
            return _instance;
        }
    }

    private System.Action onDismiss;
    private CardData currentCard;
    private PlayerController currentOwner;

    private void Awake()
    {
        _instance = this;
        // Object starts inactive in the hierarchy — no SetActive(false) needed here
    }

    public void Show(CardData card, PlayerController owner, System.Action onDismiss)
    {
        if (cardNameText == null || costText == null || shortDescText == null || fullDescText == null)
        {
            Debug.LogError("CardPreviewPanel: One or more UI components are not assigned.");
            return;
        }

        currentCard = card;
        currentOwner = owner;
        this.onDismiss = onDismiss;

        cardNameText.text = card.cardName;
        costText.text = $"Cost: {card.tokenCost.ToString()}";
        shortDescText.text = CardVisualConfig.GenerateEffectSummary(card.effects);
        fullDescText.text = card.description;

        if (background != null)
            background.color = CardVisualConfig.GetSphereColor(card.sphereType);

        var outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.effectColor = CardVisualConfig.GetPolarityColor(card.cardPolarity);
        outline.effectDistance = new Vector2(borderThickness, borderThickness);
        outline.enabled = true;

        if (sphereIconImage != null && GameServices.Instance?.sphereIconConfig != null)
        {
            Sprite icon = GameServices.Instance.sphereIconConfig.GetIconForSphere(card.sphereType);
            sphereIconImage.sprite = icon;
            sphereIconImage.enabled = icon != null;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentCard = null;
        currentOwner = null;
        onDismiss = null;
    }

    // Wired to Play button's onClick
    public void OnPlayClicked()
    {
        if (currentCard == null || currentOwner == null)
        {
            Debug.LogWarning("CardPreviewPanel: Play clicked but no card or owner set");
            return;
        }
        
        if (GameServices.Instance.cardSystem.PlayCard(currentCard, currentOwner))
            Hide();
        else
            Shake();
    }

    // Wired to Blocker button's onClick
    public void OnBlockerClicked()
    {
        var dismiss = onDismiss;
        Hide();
        dismiss?.Invoke();
    }

    
    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector2 originalPos = ((RectTransform)transform).anchoredPosition;
        float duration = 0.2f;
        float strength = 8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * strength;
            ((RectTransform)transform).anchoredPosition = originalPos + new Vector2(offsetX, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ((RectTransform)transform).anchoredPosition = originalPos;
    }
}