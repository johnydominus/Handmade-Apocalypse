using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class SoEManager
{
    //Changes a SoE level by a defined amount
    public void IncreaseEmergency(PlayerController player, EmergencyType? emergencyType, int amount)
    {
        player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType).Increase(amount);
    }

    public void DecreaseEmergency(PlayerController player, EmergencyType? emergencyType, int amount)
    {
        player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType).Decrease(amount);
    }

    //Changes a SoE level by defined amount of players' tokens
    public void SpendTokensToReduce(PlayerController player, EmergencyType? emergencyType, int tokensToSpend)
    {
        if (!player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType).stateOfEmergency.isActive)
        {
            Debug.Log($"{player.playerName} doesn't have active emergency #{emergencyType}");
            return;
        }

        if (player.SpendToken(tokensToSpend))
        {
            player.emergencies.FirstOrDefault(e => e.emergencyType == emergencyType).stateOfEmergency.PutTokens(tokensToSpend);
            Debug.Log($"{player.playerName} spent {tokensToSpend} tokens to reduce emergency #{emergencyType}");
        }
        else
        {
            Debug.Log($"{player.playerName} doesn't have enough tokens to reduce emergency.");
        }
    }
}
