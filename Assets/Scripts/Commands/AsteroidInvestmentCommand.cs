using UnityEngine;

public class AsteroidInvestmentCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private readonly int investmentAmount;

    public AsteroidInvestmentCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
        this.investmentAmount = (int)effect.value;
    }

    public void Execute()
    {
        if (player == null)
        {
            Debug.LogWarning("AsteroidInvestmentCommand requires a player");
            return;
        }

        if (investmentAmount <= 0)
        {
            Debug.LogWarning("Asteroid investment amount must be positive");
            return;
        }

        // Add investment to the player's Asteroid defense contribution
        AsteroidInvestmentManager.Instance.AddInvestment(player, investmentAmount);

        Debug.Log($"Added {investmentAmount} tokens to Asteroid defense from {player.playerName}. " +
                 $"Player total: {AsteroidInvestmentManager.Instance.GetPlayerInvestment(player)}/50. " +
                 $"Global total: {AsteroidInvestmentManager.Instance.GetTotalInvestment()}");

        // Check if defense is complete
        if (AsteroidInvestmentManager.Instance.IsDefenseComplete())
        {
            Debug.Log("Asteroid defense is complete! Threat will be deactivated.");
        }
    }

    public void Undo()
    {
        // Note: AsteroidInvestmentManager doesn't currently support removing investments
        Debug.LogWarning("Undo not implemented for AsteroidInvestmentCommand");
    }
}
