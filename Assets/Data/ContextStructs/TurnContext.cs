using UnityEngine;

public struct TurnContext
{
    public int turnNumber;
    public PlayerController player;

    public TurnContext(int turnNumber, PlayerController player)
    {
        this.turnNumber = turnNumber;
        this.player = player;
    }
}
