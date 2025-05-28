using UnityEngine;

[CreateAssetMenu(fileName = "GameServices", menuName = "Game/Services")]
public class GameServices : ScriptableObject
{
    private static GameServices _instance;
    public static GameServices Instance { get; private set; }

    public ThreatManager threatManager;
    public CardSystem cardSystem;
    public TurnManager turnManager;
    public CommandManager commandManager;
    public EffectManager effectManager;
    public InvestmentManager investmentManager;
    public SoEManager soeManager;
    public DelayedCounteractionManager delayedCounteractionManager;

    public static void Initialize(GameServices asset)
    {
        Instance = asset;
    }
}
