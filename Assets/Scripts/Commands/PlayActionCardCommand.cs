using UnityEngine;

public class PlayActionCardCommand : ICommand
{
    private readonly CardData card;
    private readonly PlayerController player;

    public PlayActionCardCommand(CardData card, PlayerController player)
    {
        this.card = card;
        this.player = player;
    }

    public void Execute()
    {
        // Check if the player has enough tokens to play the card
        ICommand command = null;
        int i = 0;

        foreach (var effect in card.effects)
        {
            Debug.Log($"Applying effect {i++} of {card.effects.Count}");
            command = new ProcessNewEffectCommand(effect, player);

            if (command != null)
                GameServices.Instance.commandManager.ExecuteCommand(command);
            else
                Debug.Log("Command creation failed - cancel execution!");
        }

        // Spend the tokens
        player.tokenManager.SpendTokens(card.tokenCost);
        player.RemoveCard(card);
    }

    public void Undo()
    {

    }
}
