using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "Game/CardLibrary")]
public class CardLibrary : ScriptableObject
{
    public List<CardData> playerActionCards;
    public List<CardData> globalEventCards;
    public List<CardData> regionEventCards;

    public CardData GetRandomCard(CardType cardType)
    {
        switch(cardType)
        {
            case CardType.PlayerAction:
                return GetRandomFrom(playerActionCards);
            case CardType.GlobalEvent:
                return GetRandomFrom(globalEventCards);
            case CardType.RegionEvent:
                return GetRandomFrom(regionEventCards);
            default:
                return null;
        }
    }

    private CardData GetRandomFrom(List<CardData> list)
    {
        if (list == null || list.Count == 0)
            return null;
        return list[Random.Range(0, list.Count)];
    }
}
