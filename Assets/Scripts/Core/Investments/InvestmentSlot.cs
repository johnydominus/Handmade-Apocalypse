using System.Collections.Generic;
using UnityEngine;

public class InvestmentSlot
{
    public string sphereName;
    public Dictionary<PlayerController, InvestorData> investors = new();

    public InvestmentSlot(string sphereName)
    {
        this.sphereName = sphereName;
    }
}
