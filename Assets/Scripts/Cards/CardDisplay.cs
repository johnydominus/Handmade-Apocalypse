using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TextMeshPro cardNameText;
    public TextMeshPro descriptionText;

    private CardData cardData;
    private PlayerController owner;
    private ThreatManager threatManager;

    void Start()
    {
        threatManager = FindFirstObjectByType<ThreatManager>();
    }

    public void Setup(CardData data, PlayerController player)
    {
        cardData = data;
        owner = player;

        cardNameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
    }

    void OnMouseDown()
    {
        TurnManager tm = FindFirstObjectByType<TurnManager>();
        tm.GetTokenUI().UpdateDisplay();

        if (tm.GetCurrentPlayer() != owner)
        {
            Debug.Log("Not your turn.");
            return;
        }

        if (owner.SpendToken(cardData.cardCost))
        {
            Debug.Log($"{owner.playerName} played card: {cardData.cardName} (Cost: {cardData.cardCost})");
            
            if (threatManager != null)
            {
                threatManager.ApplyThreatChange(cardData.threatTarget, cardData.threatChangeAmount);
                owner.savedHand.Remove(cardData);
            }

            Destroy(gameObject);
            FindFirstObjectByType<TokenUI>().UpdateDisplay();
        }
        else
        {
            Debug.Log("Not enough tokens to play this card.");
        }
    }
}
