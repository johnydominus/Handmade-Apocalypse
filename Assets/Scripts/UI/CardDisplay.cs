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

    private CardData cardData;
    private PlayerController owner;
    
    public void Setup(CardData data, PlayerController player)
    {
        cardData = data;
        owner = player;

        cardNameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
    }

    void OnMouseDown()
    {
        owner.PlayCard(cardData);
    }
}
