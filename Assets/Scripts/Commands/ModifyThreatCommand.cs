using UnityEngine;

public class ModifyThreatCommand : ICommand
{
    private readonly CardData card;
    private readonly PlayerController player;
    private int originalThreatLevel;

    public ModifyThreatCommand(CardData card, PlayerController player)
    {
        this.card = card;
        this.player = player;
    }

    public void Execute()
    {
        var threatManager = GameServices.Instance.threatManager;
        originalThreatLevel = threatManager.GetThreatValue(card.targetThreat);

        threatManager.ApplyThreatChange(card.targetThreat, card.threatModifier);
        player.tokenManager.SpendTokens(card.tokenCost);
    }

    public void Undo()
    {
        var threatManager = GameServices.Instance.threatManager;
        int delta = originalThreatLevel - threatManager.GetThreatValue(card.targetThreat);
        threatManager.ApplyThreatChange(card.targetThreat, delta);
        player.tokenManager.AddTokens(card.tokenCost);
    }
}
