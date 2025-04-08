using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButton : MonoBehaviour
{
    public TurnManager turnManager;

    public void OnEndTurnPressed()
    {
        turnManager.EndTurn();
    }
}
