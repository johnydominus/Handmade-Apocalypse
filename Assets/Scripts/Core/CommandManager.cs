using System.Collections.Generic;
using UnityEngine;

public class CommandManager
{
    private Stack<ICommand> executedCommands = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        executedCommands.Push(command);
    }

    public void Undo()
    {
        if (executedCommands.Count > 0)
        {
            var command = executedCommands.Pop();
            command.Undo();
        }
    }
}
