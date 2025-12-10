using UnityEngine;

public class ModifyPlayerTokensCommand : ICommand
{
    private readonly Effect effect;
    private readonly PlayerController player;
    private readonly int originalTokens;
    private readonly int turnsRemaining;

    public ModifyPlayerTokensCommand(Effect effect, PlayerController player)
    {
        this.effect = effect;
        this.player = player;
        this.originalTokens = player.tokenManager.GetTokens();
        this.turnsRemaining = effect.effectTiming.duration;
    }

    public void Execute()
    {
        int currentTokens = player.tokenManager.GetTokens();
        int newTokens = currentTokens;

        switch (effect.effectType)
        {
            case EffectType.Add:
                // Add or subtract tokens
                newTokens = currentTokens + (int)effect.value;
                Debug.Log($"Adding {effect.value} tokens to {player.playerName}. {currentTokens} -> {newTokens}");
                break;

            case EffectType.Multiply:
                // Multiply tokens
                newTokens = Mathf.FloorToInt(currentTokens * effect.value);
                Debug.Log($"Multiplying tokens by {effect.value}. {currentTokens} -> {newTokens}");
                break;

            case EffectType.Block:
                // Set tokens to 0 (complete block)
                newTokens = 0;
                Debug.Log($"Blocking tokens for {player.playerName}. {currentTokens} -> {newTokens}");
                break;
        }

        // Apply the change (this now supports negative values)
        player.tokenManager.SetTokens(newTokens);

        // Register the effect if it has duration (for temporary effects)
        if (effect.effectTiming.effectTimingType != EffectTimingType.Immediate)
        {
            GameServices.Instance.effectManager.RegisterEffect(effect, player);
        }

        // Notify UI
        GameEvents.OnTokensChanged.Raise(player);

        Debug.Log($"Modified tokens for {player.playerName}. New balance: {player.tokenManager.GetTokens()}");
    }

    public void Undo()
    {
        // Restore original token count
        player.tokenManager.SetTokens(originalTokens);
        GameEvents.OnTokensChanged.Raise(player);
        Debug.Log($"Undid token modification for {player.playerName}. Restored to {originalTokens}");
    }
}
