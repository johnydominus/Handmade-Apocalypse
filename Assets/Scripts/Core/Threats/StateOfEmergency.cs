using UnityEngine;

public class StateOfEmergency
{
    public bool isActive { get; private set; }
    public int tokensPut { get; private set; }

    private Emergency parent;
    private int tokensToDeactive = 12;

    public StateOfEmergency(Emergency parent)
    {
        this.parent = parent;
        isActive = false;
        tokensPut = 0;
    }

    public void Activate()
    {
        isActive = true;
        GameEvents.OnSoEActivated.Raise(new SoEContext(parent.emergencyType, parent.player));
        Debug.Log($"⚠️ Emergency #{parent.emergencyType} activated!");
    }

    public void PutTokens(int amount)
    {
        tokensPut += amount;
        if (tokensPut >= tokensToDeactive)
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        isActive = false;
        parent.Set(3);
    }
}
