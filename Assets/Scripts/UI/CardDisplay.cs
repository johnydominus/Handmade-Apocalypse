using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Setup(CardData data, PlayerController player)
    {
        cardData = data;
        owner = player;

        cardNameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        cardCostText.text = cardData.tokenCost.ToString();

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
            if (icon != null)
            {
                sphereIconRenderer.sprite = icon;
                sphereIconRenderer.enabled = true;
            }
            else
            {
                sphereIconRenderer.enabled = false;
            }
        }
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    void OnMouseDown()
    {
        Debug.Log($"{owner.playerName} clicked {cardData.cardName} card...");

        if (cardData == null)
        {
            Debug.Log($"...but the card data is null.");
            return;
        }

        if (!GameServices.Instance.cardSystem.PlayCard(cardData, owner))
        {
            Debug.Log($"...but doesn't have enough tokens.");
            Shake();
            return;
        }
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = transform.localPosition;
        float shakeDuration = 0.2f;
        float shakeStrength = 0.5f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeStrength;
            transform.localPosition = originalPos + new Vector3(offsetX, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
