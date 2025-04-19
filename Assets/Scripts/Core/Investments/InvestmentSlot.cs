using System.Collections.Generic;
using UnityEngine;

public class InvestmentSlot
{
    public SphereType sphereName;
    public Dictionary<PlayerController, InvestorData> investors = new();

    public InvestmentSlot(SphereType sphereName)
    {
        this.sphereName = sphereName;
    }
}
