using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldDisplayer : MonoBehaviour
{
    List<List<Card>> cards;
    List<List<Vector3>> cardPositions;
    List<CardManager.CardType> cardTypesPossessed;

    float cardInitialPos = 5.0f;
    float spaceBetweenColumn = 0.3f;

    int cardMaxNumberPerColumn = 10;

    [SerializeField] Vector2 CardDisplayArea = Vector2.one;

    void Start() {
        cards = new List<List<Card>>();
        cardPositions = new List<List<Vector3>>();
        cardTypesPossessed = new List<CardManager.CardType>();
    }

    private void FixedUpdate() {
        for (int i = 0; i < cardPositions.Count; i++) {
            for (int j = 0; j < cardPositions[i].Count; j++) {
                cards[i][j].customTransform.localPosition = cardPositions[i][j];
            }
        }
    }

    private void AddCardToField(Card card) {
        card.customTransform.parent = transform;

        if (cardTypesPossessed.Contains(card.cardType)) {
            int index = cardTypesPossessed.FindIndex(x => x == card.cardType);
            cardPositions[index].Add(new Vector3());
        }
        else {
            if (cardTypesPossessed.Count == 0) {
                cardTypesPossessed.Add(card.cardType);
                cardPositions.Add(new List<Vector3>());
                cardPositions[0].Add(new Vector3());
                return;
            }

            for (int i = 0; i < cardTypesPossessed.Count; i++) {
                if (cardTypesPossessed[i] < card.cardType) {
                    if (i == cardTypesPossessed.Count - 1) {
                        cardTypesPossessed.Add(card.cardType);
                        cardPositions.Add(new List<Vector3>());
                        cardPositions[cardPositions.Count - 1].Add(new Vector3());
                        return;
                    }
                    continue;
                }

                cardTypesPossessed.Insert(i, card.cardType);
                cardPositions.Insert(i, new List<Vector3>());
                cardPositions[i].Add(new Vector3());
                return;
            }
        }

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                Card.raceNmb * Card.cardWidth + (Card.raceNmb - 1) * spaceBetweenColumn + Card.cardWidth,
                0.0f,
                cardInitialPos * 2 + Card.cardLength)
            );
    }
}
