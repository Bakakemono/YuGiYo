using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    List<Transform> cards = new List<Transform>();
    float cardThickness = 0.01f;
    Vector2 cardDimension = new Vector2(1.2f, 2.2f);

    BoxCollider boxCollider;

    bool playerTurn = true;

    void Start() {
        Card[] currentCards = GetComponentsInChildren<Card>();
        foreach (Card card in currentCards) {
            cards.Add(card.GetComponent<Transform>());
        }

        for (int i = 0; i < cards.Count; i++) {
            cards[i].localPosition = new Vector3(0, 0, cardThickness / 2.0f + (cards.Count - i) * (cardThickness));
        }

        boxCollider = GetComponent<BoxCollider>();
        CalculateHitBoxSize();
    }

    void CalculateHitBoxSize() {
        boxCollider.size = new Vector3(cardDimension.x, cardDimension.y, (cardThickness) * cards.Count);
        boxCollider.center = new Vector3(0, 0, (cardThickness) * cards.Count / 2.0f);
    }

    public Transform DrawCard() {
        Transform DrawedCard = cards[0];
        cards.RemoveAt(0);
        CalculateHitBoxSize();
        return DrawedCard;
    }
}
