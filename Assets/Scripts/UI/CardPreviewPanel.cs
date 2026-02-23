using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI shortDescText;
    [SerializeField] private TextMeshProUGUI fullDescText;
    [SerializeField] private Image background;
    [SerializeField] private Image sphereIconImage;
    [SerializeField] private float borderThickness = 6f;

    public static CardPreviewPanel Instance { get; private set; }

    private System.Action onDismiss;
    private CardData currentCard;
    private PlayerController currentOwner;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(CardData card, PlayerController owner, System.Action onDismiss)
    {
        if (cardNameText == null || costText == null || shortDescText == null || fullDescText == null)
        {
            Debug.LogError("CardPreviewPanel: One or more UI components are not assigned.");
            return;
        }

        currentCard = card;
        currentOwner = owner;
        this.onDismiss = onDismiss;

        cardNameText.text = card.cardName;
        costText.text = $"Cost: {card.tokenCost.ToString()}";
        shortDescText.text = CardVisualConfig.GenerateEffectSummary(card.effects);
        fullDescText.text = card.description;

        if (background != null)
            background.color = CardVisualConfig.GetSphereColor(card.sphereType);

        var outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.effectColor = CardVisualConfig.GetPolarityColor(card.cardPolarity);
        outline.effectDistance = new Vector2(borderThickness, borderThickness);
        outline.enabled = true;

        if (sphereIconImage != null && GameServices.Instance?.sphereIconConfig != null)
        {
            Sprite icon = GameServices.Instance.sphereIconConfig.GetIconForSphere(card.sphereType);
            sphereIconImage.sprite = icon;
            sphereIconImage.enabled = icon != null;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentCard = null;
        currentOwner = null;
        onDismiss = null;
    }

    // Wired to Play button's onClick
    public void OnPlayClicked()
    {
        if (currentCard == null || currentOwner == null)
        {
            Debug.LogWarning("CardPreviewPanel: Play clicked but no card or owner set");
            return;
        }
        
        if (!GameServices.Instance.cardSystem.PlayCard(currentCard, currentOwner))
        {
            Debug.LogWarning("Not enough tokens to play this card!");
            // oprtionally flash the cost text red here
        }
        // On success: CardDisplay.OnDestroy fires -> Hide()
    }

    // Wired to Blocker button's onClick
    public void OnBlockerClicked()
    {
        Hide();
        onDismiss?.Invoke();
    }
}