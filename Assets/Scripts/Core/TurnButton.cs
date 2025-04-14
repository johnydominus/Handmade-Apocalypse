using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButton : MonoBehaviour
{
    public TurnManager turnManager;

    public void OnEndTurnPressed()
    {
        GameServices.Instance.turnManager.EndTurn();
    }
}
