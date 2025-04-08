using UnityEngine;
using TMPro;

public class InvestmentForecastDisplay : MonoBehaviour
{
    public int sphereIndex; // 0 = Astro, 1 = Diplo, 2 = Med

    public TextMeshProUGUI oneTurnText;
    public TextMeshProUGUI twoTurnText;
    public TextMeshProUGUI threeTurnText;

    //Updates displayed amount of upcoming dividends
    public void UpdateForecast(PlayerController sphereOwner, PlayerController investor)
    {
        int investorIndex = (investor == TurnManager.Instance.player1) ? 0 : 1;
        int invested = sphereOwner.incomingInvestments[sphereIndex, investorIndex];

        int shortTerm = invested / 3;
        int leftover = invested % 3;

        oneTurnText.text = shortTerm.ToString();
        twoTurnText.text = shortTerm.ToString();
        threeTurnText.text = (shortTerm + leftover).ToString();
    }

}
