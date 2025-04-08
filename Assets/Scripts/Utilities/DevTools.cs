using UnityEngine;
using TMPro;

public class DevTools : MonoBehaviour
{
    public GameObject panel;
    public SoEManager soeManager;
    public RegionUI regionUI;

    public PlayerController player1;
    public PlayerController player2;
    public PlayerController targetPlayer { get; private set; }
    public TextMeshProUGUI playerNameLabel; // ← assign in Inspector

    private PlayerController[] players;
    private int currentIndex = 0;

    void Awake()
    {
        players = new[] { player1, player2 };
        SetTargetPlayer(players[currentIndex]);
    }

    public void SetTargetPlayer(PlayerController player)
    {
        targetPlayer = player;
        playerNameLabel.text = player.playerName;
        Debug.Log($"DevTools: Switched to {player.playerName}");
    }

    public void NextPlayer()
    {
        currentIndex = (currentIndex + 1) % players.Length;
        SetTargetPlayer(players[currentIndex]);
    }

    public void PreviousPlayer()
    {
        currentIndex = (currentIndex - 1 + players.Length) % players.Length;
        SetTargetPlayer(players[currentIndex]);
    }

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void RaiseEmergency(int index)
    {
        if (targetPlayer != null)
        {
            soeManager.ModifyEmergency(targetPlayer, index, +1);
            regionUI.UpdateEmergencyBars();
        }
    }

    public void LowerEmergency(int index)
    {
        if (targetPlayer != null)
        {
            soeManager.ModifyEmergency(targetPlayer, index, -1);
            regionUI.UpdateEmergencyBars();
        }
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}