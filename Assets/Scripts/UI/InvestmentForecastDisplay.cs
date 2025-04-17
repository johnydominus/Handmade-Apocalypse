using UnityEngine;
using TMPro;

public class InvestmentForecastDisplay : MonoBehaviour
{
    public int sphereIndex; // 0 = Astro, 1 = Diplo, 2 = Med

    public TextMeshProUGUI oneTurnText;
    public TextMeshProUGUI twoTurnText;
    public TextMeshProUGUI threeTurnText;

    private void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
        GameEvents.OnTokensChanged.RegisterListener(OnTokensChanged);
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
        GameEvents.OnTokensChanged.UnregisterListener(OnTokensChanged);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        UpdateForecast(turnContext.player, turnContext.player);
    }

    private void OnTokensChanged(PlayerController player)
    {
        UpdateForecast(player, player);
    }

    public void UpdateForecast(PlayerController owner, PlayerController investor)
    {
        if (owner == null)
        {
            Debug.Log($"Owner is null!");
            return;
        }
        Debug.Log($"sphereIndex = {sphereIndex.ToString()}");
        if (owner.investments[sphereIndex] == null)
        {
            Debug.Log($"Investment slot is null for sphereIndex {sphereIndex}");
            return;
        }
        var slot = owner.investments[sphereIndex];
        if (!slot.investors.ContainsKey(investor)) return;

        var data = slot.investors[investor];

        int fastDivs = data.investedTokens / 3;
        int[] slow = new int[3];

        foreach (int timer in data.slowDividendTimers)
        {
            if (timer >= 1 && timer <= 3)
                slow[timer - 1]++;
        }

        oneTurnText.text = (fastDivs + slow[0]).ToString();
        twoTurnText.text = slow[1].ToString();
        threeTurnText.text = slow[2].ToString();

        Debug.Log($"Forecast updated for {owner.name}");
    }
}
