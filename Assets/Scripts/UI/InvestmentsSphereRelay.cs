using UnityEngine;
using TMPro;
using Mono.Cecil;

public class InvestmentSphereRelay : MonoBehaviour
{
    public TokenUI tokenUI;
    public TextMeshProUGUI amountLabel; // 👈 assign in Inspector
    public TextMeshProUGUI sphereName;
    public InvestmentForecastDisplay forecastDisplay;

    [System.NonSerialized] private InvestmentManager investmentManager;
    private PlayerController investor;
    private PlayerController sphereOwner;

    public int sphereIndex;

    public void OnEnable()
    {
        GameEvents.OnTurnStarted.RegisterListener(OnTurnStarted);
    }

    public void OnDisable()
    {
        GameEvents.OnTurnStarted.UnregisterListener(OnTurnStarted);
    }

    private void OnTurnStarted(TurnContext turnContext)
    {
        SetContext(turnContext.player, turnContext.player);
        UpdateAmountText();
    }

    public void Start()
    {
        investmentManager = GameServices.Instance.investmentManager;
    }

    public void SetContext(PlayerController sphereOwner, PlayerController investor)
    {
        if (sphereOwner == null || investor == null)
        {
            Debug.Log("Sphere owner or investor is null!");
        }
        this.sphereOwner = sphereOwner;
        this.investor = investor;

        Debug.Log("The context for InvestmentSphereRelay is set!");
    }

    //Adds a token to a respective developement sphere
    public void OnPlusClicked()
    {
        investmentManager.InvestToken(investor, sphereOwner, sphereIndex);
        UpdateAmountText();
        Debug.Log("Plus clicked");
        tokenUI.UpdateDisplay(investor);
    }

    //Withdraws a token from a respective developement sphere
    public void OnMinusClicked()
    {
        investmentManager.WithdrawToken(investor, sphereOwner, sphereIndex);
        UpdateAmountText();
        Debug.Log("Minus clicked");
        tokenUI.UpdateDisplay(investor);
        Debug.Log("Forecast updated for sphere " + sphereIndex);
    }

    public void UpdateAmountText()
    {
        var slot = sphereOwner.investments[sphereIndex];

        if (slot.investors.TryGetValue(investor, out var data))
        {
            amountLabel.text = data.investedTokens.ToString();
        }
        else
        {
            amountLabel.text = "0";
        }
    }
}
