using TMPro;
using UnityEngine;

public class TurnHeaderUI : MonoBehaviour
{
    public TextMeshProUGUI playersName;
    public TextMeshProUGUI turnNumber;

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
        playersName.text = $"{turnContext.player.playerName}'s turn";
        turnNumber.text = turnContext.turnNumber.ToString();
    }
}
