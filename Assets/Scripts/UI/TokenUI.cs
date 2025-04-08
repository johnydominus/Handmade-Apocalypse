using UnityEngine;
using TMPro;

public class TokenUI : MonoBehaviour
{
    public TextMeshProUGUI tokenText;
    public PlayerController player;

    public void UpdateDisplay()
    {
        if (tokenText == null || player == null)
        {
            Debug.LogWarning("TokenUI not fully initialized!");
            return;
        }

        tokenText.text = $"Tokens: {player.tokens}";
    }
}
