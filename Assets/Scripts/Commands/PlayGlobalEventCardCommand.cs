using UnityEngine;

public class PlayGlobalEventCardCommand : ICommand
{
    private readonly CardData card;

    public PlayGlobalEventCardCommand(CardData card)
    {
        this.card = card;
    }

    public void Execute()
    {
        // Check if the player has enough tokens to play the card
        ICommand command = null;
        int i = 0;

        foreach (var effect in card.effects)
        {
            Debug.Log($"Applying effect {i++} of {card.effects.Count}");
            command = new ProcessNewEffectCommand(effect, null, card);

            if (command != null)
                GameServices.Instance.commandManager.ExecuteCommand(command);
            else
                Debug.Log("Command creation failed - cancel execution!");
        }
    }

    public void Undo()
    {

    }
}
