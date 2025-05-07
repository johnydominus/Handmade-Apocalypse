using UnityEngine;
#nullable enable

public class StartTurnCommand : ICommand
{
    TurnManager turnManager;

    public StartTurnCommand(TurnManager turnManager)
    {
        this.turnManager = turnManager;
    }

    public void Execute()
    {
        turnManager.StartTurn();
    }

    public void Undo()
    {

    }
}
