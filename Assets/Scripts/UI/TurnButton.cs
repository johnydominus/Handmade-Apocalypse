using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    [SerializeField] private Text buttonText;

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        TurnManager turnManager = GameServices.Instance.turnManager;
        List<PlayerController> players = turnManager.GetAllPlayers();
        PlayerController currentPlayer = turnManager.GetCurrentPlayer();

        int currentPlayerIndex = players.IndexOf(currentPlayer);
        bool isLastPlayer = (currentPlayerIndex == players.Count - 1);

        buttonText.text = isLastPlayer ? "End Turn" : "Next Player";
    }

    public void OnEndTurnPressed()
    {
        GameServices.Instance.turnManager.EndTurn();
    }
}
