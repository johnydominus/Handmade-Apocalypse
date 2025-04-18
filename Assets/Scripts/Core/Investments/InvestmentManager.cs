﻿using UnityEngine;

public class InvestmentManager
{
    public void InvestToken(PlayerController investor, PlayerController target, int sphereIndex)
    {
        if (!investor.SpendToken(1)) return;

        var slot = target.investments[sphereIndex];

        if (!slot.investors.ContainsKey(investor))
            slot.investors[investor] = new InvestorData();

        slot.investors[investor].investedTokens++;

        int leftover = slot.investors[investor].investedTokens % 3;

        SyncSlowDividends(slot.investors[investor]);

        GameEvents.OnTokensChanged.Raise(investor);
    }

    public void WithdrawToken(PlayerController investor, PlayerController target, int sphereIndex)
    {
        var slot = target.investments[sphereIndex];
        if (!slot.investors.ContainsKey(investor) || slot.investors[investor].investedTokens == 0) return;

        slot.investors[investor].investedTokens--;

        int leftover = slot.investors[investor].investedTokens % 3;

        SyncSlowDividends(slot.investors[investor]);

        investor.tokenManager.AddTokens(1);
        GameEvents.OnTokensChanged.Raise(investor);
    }

    private void SyncSlowDividends(InvestorData data)
    {
        int leftover = data.investedTokens % 3;

        // Add missing slow timers
        while (data.slowDividendTimers.Count < leftover)
            data.slowDividendTimers.Add(3);

        // Remove excess slow timers
        while (data.slowDividendTimers.Count > leftover)
            data.slowDividendTimers.RemoveAt(data.slowDividendTimers.Count - 1);
    }


    public void TickInvestments()
    {
        foreach (var player in GameServices.Instance.turnManager.GetAllPlayers())
        {
            foreach (var slot in player.investments)
            {
                foreach (var entry in slot.investors)
                {
                    var investor = entry.Key;
                    var data = entry.Value;

                    // Count fast dividends
                    int fastTokens = data.investedTokens / 3;

                    // Tick down each slow timer
                    int slowTokens = 0;
                    for (int i = data.slowDividendTimers.Count - 1; i >= 0; i--)
                    {
                        data.slowDividendTimers[i]--;
                        if (data.slowDividendTimers[i] <= 0)
                        {
                            slowTokens++;
                            data.slowDividendTimers.RemoveAt(i);
                        }
                    }
                    // Pay dividends
                    if (slowTokens + fastTokens > 0)
                    {
                        investor.tokenManager.AddTokens(slowTokens + fastTokens);
                        GameEvents.OnTokensChanged.Raise(investor);
                        Debug.Log($"[Dividends] {investor.playerName} received {fastTokens} fast + {slowTokens} slow tokens.");
                    }

                    // Add missing slow timers
                    int leftover = data.investedTokens % 3;
                    while (data.slowDividendTimers.Count < leftover)
                        data.slowDividendTimers.Add(3);
                }
            }
        }
    }
}
