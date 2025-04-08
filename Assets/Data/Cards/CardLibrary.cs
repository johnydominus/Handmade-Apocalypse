using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "Game/CardLibrary")]
public class CardLibrary : ScriptableObject
{
    public List<CardData> allCards;
}
