using UnityEngine;

public struct SoEContext
{
    public EmergencyType? soeType;
    public PlayerController player;

    public SoEContext(EmergencyType? soeType, PlayerController player)
    {
        this.soeType = soeType;
        this.player = player;
    }
}
