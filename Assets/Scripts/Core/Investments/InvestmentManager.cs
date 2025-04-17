using UnityEngine;

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

        if (leftover == 0 && slot.investors[investor].slowDividendTimers.Count < leftover)
            slot.investors[investor].slowDividendTimers.Add(0);

        GameEvents.OnTokensChanged.Raise(investor);
    }

    public void WithdrawToken(PlayerController investor, PlayerController target, int sphereIndex)
    {
        var slot = target.investments[sphereIndex];
        if (!slot.investors.ContainsKey(investor) || slot.investors[investor].investedTokens == 0) return;

        slot.investors[investor].investedTokens--;

        int leftover = slot.investors[investor].investedTokens % 3;

        if (slot.investors[investor].slowDividendTimers.Count > leftover)
            slot.investors[investor].slowDividendTimers.RemoveAt(leftover);

        investor.tokenManager.AddTokens(1);
        GameEvents.OnTokensChanged.Raise(investor);
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

                    int fastTokens = data.investedTokens / 3;
                    if (fastTokens > 0)
                        investor.tokenManager.AddTokens(fastTokens);

                    // Tick down each slow timer
                    for (int i = data.slowDividendTimers.Count - 1; i >= 0; i--)
                    {
                        data.slowDividendTimers[i]--;
                        if (data.slowDividendTimers[i] <= 0)
                        {
                            investor.tokenManager.AddTokens(1);
                            data.slowDividendTimers.RemoveAt(i);
                        }
                    }
                    GameEvents.OnTokensChanged.Raise(investor);
                }
            }
        }
    }
}
