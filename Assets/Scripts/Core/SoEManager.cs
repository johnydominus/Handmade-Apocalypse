using UnityEngine;

public class SoEManager : MonoBehaviour
{
    //Changes a SoE level by a defined amount
    public void ModifyEmergency(PlayerController player, int index, int amount)
    {
        player.emergencyLevels[index] += amount;
        player.emergencyLevels[index] = Mathf.Clamp(player.emergencyLevels[index], 0, 10);

        // Activate if threshold hit
        if (player.emergencyLevels[index] >= 10 && !player.isEmergencyActive[index])
        {
            player.isEmergencyActive[index] = true;
            Debug.Log($"⚠️ {player.playerName} enters emergency #{index}!");
        }

        // Deactivate if lowered
        if (player.emergencyLevels[index] < 10 && player.isEmergencyActive[index])
        {
            player.isEmergencyActive[index] = false;
            Debug.Log($"✅ {player.playerName} resolved emergency #{index}.");
        }
    }

    //Changes a SoE level by defined amount of players' tokens 
    public void SpendTokensToReduce(PlayerController player, int index, int tokensToSpend)
    {
        if (player.SpendToken(tokensToSpend))
        {
            ModifyEmergency(player, index, -tokensToSpend);
        }
        else
        {
            Debug.Log($"{player.playerName} doesn't have enough tokens to reduce emergency.");
        }
    }
}
