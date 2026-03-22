using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.GameCenter;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private TextMeshPro cardCostText;
    [SerializeField] private SpriteRenderer borderRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer sphereIconRenderer;

    private CardData cardData;
    private PlayerController owner;
    private Camera mainCamera;

    private static CardDisplay currentlyExpandedCard = null;
    private static float dismissTime = -1f;
    private const float dismissIgnoreWindow = 0.15f;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Setup(CardData data, PlayerController player)
    {
        cardData = data;
        owner = player;

        cardNameText.text = cardData.cardName;
//        descriptionText.text = cardData.description;
        cardCostText.text = cardData.tokenCost.ToString();

        // Hand mode: show short description
        RefreshDescriptionText();

        // Apply sphere background color
        if (backgroundRenderer != null)
            backgroundRenderer.color = CardVisualConfig.GetSphereColor(cardData.sphereType);

        // Apply polarity border color (only for GE/RE cards, not Player Actions)
        if (borderRenderer != null && cardData.cardType != CardType.PlayerAction)
            borderRenderer.color = CardVisualConfig.GetPolarityColor(cardData.cardPolarity);

        // Apply sphere icon
        if (sphereIconRenderer != null && GameServices.Instance != null && GameServices.Instance.sphereIconConfig != null)
        {
            Sprite icon = GameServices.Instance.sphereIconConfig.GetIconForSphere(cardData.sphereType);
            sphereIconRenderer.sprite = icon;
            sphereIconRenderer.enabled = icon != null;
        }
    }

    public CardData GetCardData() => cardData;

    void OnMouseDown()
    {
        Debug.Log($"{owner.playerName} clicked {cardData.cardName} card...");

        if (cardData == null)
        {
            Debug.Log($"...but the card data is null.");
            return;
        }

        // Ignore clickes right after a dissmiss (blocker click propagates to world-space)
        if (Time.time - dismissTime < dismissIgnoreWindow) return;

        // A card is already expanded (this one or another) - do nothing
        if (currentlyExpandedCard != null)
        {
            Debug.Log($"...but another card is already expanded.");
            return;
        }

        Expand();
    }

    private string GetShortDesc()
    {
        if (!string.IsNullOrEmpty(cardData.shortDescription))
            return cardData.shortDescription;
        return CardVisualConfig.GenerateEffectSummary(cardData.effects);
    }

    private void Expand()
    {
        if (cardData == null) return;

        if (CardPreviewPanel.Instance == null)
        {
            Debug.LogError("CardPreviewPanel not found in scene. Add it as a child of the Canvas.");
            return;
        }

        currentlyExpandedCard = this;

        CardPreviewPanel.Instance.Show(cardData, owner, () => {
            if (currentlyExpandedCard == this) currentlyExpandedCard = null;
        });
    }

    private void RefreshDescriptionText()
    {
        if (cardData == null) return;
        descriptionText.text = GetShortDesc();
    }

    private void OnDestroy()
    {
        if (currentlyExpandedCard == this)
            currentlyExpandedCard = null;
    }
}
