using UnityEngine;

public class CardDrawTester : MonoBehaviour
{
    [SerializeField] private CardLibrary library;

    public void DrawRandomPlayerAction()
    {
        var card = library.GetRandomCard(CardType.PlayerAction);
        GameEvents.OnCardDrawn.Raise(card);
    }
}
