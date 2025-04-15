using UnityEngine;
using TMPro;

public class InvestmentButtonRelay : MonoBehaviour
{
    public InvestmentManager investmentManager;
    public PlayerController sphereOwner;
    public TokenUI tokenUI;
    public InvestmentForecastDisplay forecastDisplay;
    public TextMeshProUGUI amountLabel; // 👈 assign in Inspector
    private PlayerController investor;

    public int sphereIndex;

    public void SetContext(PlayerController sphereOwner, PlayerController investor)
    {
        this.sphereOwner = sphereOwner;
        this.investor = investor;
    }

    //Adds a token to a respective developement sphere
    public void OnPlusClicked()
    {
//        investmentManager.InvestToken(investor, sphereOwner, sphereIndex);
        UpdateAmountText();
        Debug.Log("Plus clicked");
        tokenUI.UpdateDisplay(investor);
//        forecastDisplay.UpdateForecast(sphereOwner, investor);
        Debug.Log("Forecast updated for sphere " + sphereIndex);
    }

    //Withdraws a token from a respective developement sphere
    public void OnMinusClicked()
    {
//        investmentManager.WithdrawToken(investor, sphereOwner, sphereIndex);
        UpdateAmountText();
        Debug.Log("Minus clicked");
        tokenUI.UpdateDisplay(investor);
//        forecastDisplay.UpdateForecast(sphereOwner, investor);
        Debug.Log("Forecast updated for sphere " + sphereIndex);
    }
    
    public void UpdateAmountText()
    {
//        int investorIndex = (investor == TurnManager.Instance.player1) ? 0 : 1;
//        int value = sphereOwner.incomingInvestments[sphereIndex, investorIndex];
//        amountLabel.text = value.ToString();
    }
}
