using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUIController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform handAnchor; // empty GameObject to position the hand

    public void DisplayHand(PlayerController player)
    {
        ClearHand();

        List<CardData> hand = player.GetHand();
        int count = hand.Count;

        float baseSpacing = 2.2f;
        float spacing = count <= 3 ? baseSpacing : (baseSpacing * 3) / count; // shrink as hand grows
        float startX = -((count - 1) * spacing) / 2f;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = handAnchor.position + new Vector3(startX + (i * spacing), 0, 0);
            GameObject card = Instantiate(cardPrefab, spawnPos, Quaternion.identity, handAnchor);
            card.GetComponent<CardDisplay>().Setup(hand[i], player);
        }
    }

    public void ClearHand()
    {
        foreach (Transform child in handAnchor)
        {
            Destroy(child.gameObject);
        }
    }

    /* OLD IMPLEMENTATION
    public void DrawHand(PlayerController player)
    {
        ClearHand();

        // First turn = draw 3 fresh
        if (!player.hasPlayedOnce)
        {
            for (int i = 0; i < 3; i++)
            {
                player.savedHand.Add(GetRandomCard());
            }
            player.hasPlayedOnce = true;
        }
        else
        {
            // Every turn after: draw 1 extra
            player.savedHand.Add(GetRandomCard());
        }

        int count = player.savedHand.Count;

        float baseSpacing = 2.2f;
        float spacing = count <= 3 ? baseSpacing : (baseSpacing * 3) / count; // shrink as hand grows
        float startX = -((count - 1) * spacing) / 2f;

        // Display cards
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = handAnchor.position + new Vector3(startX + (i * spacing), 0, 0);
            GameObject card = Instantiate(cardPrefab, spawnPos, Quaternion.identity, handAnchor);
            card.GetComponent<CardDisplay>().Setup(player.savedHand[i], player);
        }
    }

    public void ClearHand()
    {
        foreach (Transform child in handAnchor)
            Destroy(child.gameObject);
    }

    private CardData GetRandomCard()
    {
        return library.playerActionCards[Random.Range(0, library.playerActionCards.Count)];
    }
    */
}
