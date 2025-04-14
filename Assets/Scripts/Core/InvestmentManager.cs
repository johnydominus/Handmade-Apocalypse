using UnityEngine;

public class InvestmentManager : MonoBehaviour
{

    /* OLD IMPLEMENTATION
    public PlayerController player1; //Assigned in Inspector
    public PlayerController player2; //Assigned in Inspector


    public void InvestToken(PlayerController investingPlayer, PlayerController targetPlayer, int sphereIndex)
    {
        if (investingPlayer.SpendToken(1))
        {
            int investingIndex = (investingPlayer == TurnManager.Instance.player1) ? 0 : 1;
            targetPlayer.incomingInvestments[sphereIndex, investingIndex]++;
            Debug.Log($"{investingPlayer.playerName} invested in {targetPlayer.playerName}'s sphere #{sphereIndex}");
        }
    }

    public void WithdrawToken(PlayerController investingPlayer, PlayerController targetPlayer, int sphereIndex)
    {
        int investingIndex = (investingPlayer == TurnManager.Instance.player1) ? 0 : 1;

        if (targetPlayer.incomingInvestments[sphereIndex, investingIndex] > 0)
        {
            targetPlayer.incomingInvestments[sphereIndex, investingIndex]--;
            investingPlayer.tokens += 1;
            Debug.Log($"{investingPlayer.playerName} withdrew 1 token from sphere #{sphereIndex}");
        }
    }

    //Controls dividends return
    public void TickInvestments()
    {
        PlayerController[] players = { player1, player2 };

        foreach (PlayerController sphereOwner in players)
        {
            for (int sphereIndex = 0; sphereIndex < 3; sphereIndex++)
            {
                for (int investorIndex = 0; investorIndex < 2; investorIndex++)
                {
                    int investedTokens = sphereOwner.incomingInvestments[sphereIndex, investorIndex];
                    PlayerController investor = players[investorIndex];

                    // 1 token per turn for every 3 tokens
                    int fastPayout = investedTokens / 3;
                    investor.tokens += fastPayout;

                    // Track slow payouts
                    int leftover = investedTokens % 3;
                    if (leftover > 0)
                    {
                        investor.slowDividendCounters[sphereIndex, sphereOwner == player1 ? 0 : 1]++;
                        if (investor.slowDividendCounters[sphereIndex, sphereOwner == player1 ? 0 : 1] >= 3)
                        {
                            investor.tokens += leftover;
                            investor.slowDividendCounters[sphereIndex, sphereOwner == player1 ? 0 : 1] = 0;
                        }
                    }
                }
            }
        }
    }
    */
}
