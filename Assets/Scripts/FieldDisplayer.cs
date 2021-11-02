using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldDisplayer : MonoBehaviour
{
    List<List<Card>> cards;
    List<List<Vector3>> cardPositions;
    List<CardManager.CardType> cardTypesPossessed;
    [SerializeField] List<CardManager.CardType> raceTypePossessed;

    float cardInitialPos = 5.0f;
    float spaceBetweenColumn = 0.3f;

    int cardMaxNumberPerColumn = 10;

    int typeCurrentlyOnBoard = 0;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float decalBetweenCard = 0.25f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float secretCardAdvance = 0.75f;

    [SerializeField] Vector2 CardDisplayArea = Vector2.one;


    bool addCarts = true;
    void Start() {
        cards = new List<List<Card>>();
        cardPositions = new List<List<Vector3>>();
        cardTypesPossessed = new List<CardManager.CardType>();
        raceTypePossessed = new List<CardManager.CardType>();
    }

    private void Update() {
        DrawPileManager drawPileManager = FindObjectOfType<DrawPileManager>();

        if (Input.GetKeyDown(KeyCode.A)) {
            AddCardToField(drawPileManager.DrawCard());
        }

        for (int i = 0; i < cardPositions.Count; i++) {
            for (int j = 0; j < cardPositions[i].Count; j++) {
                cards[i][j].customTransform.localPosition = cardPositions[i][j];
                cards[i][j].customTransform.localRotation = Quaternion.Euler(89.0f, 0.0f, 0.0f);
            }
        }
    }

    private void AddCardToField(Card card) {
        if(card.cardType == CardManager.CardType.BEER) {
            Destroy(card.gameObject);
            return;
        }

        bool AlreadyAdded = false;
        if (!cardTypesPossessed.Contains(card.cardType - Card.raceNmb) && !cardTypesPossessed.Contains(card.cardType)) {
            CardManager.CardType raceType = (card.cardType - Card.raceNmb < 0 ? card.cardType : card.cardType - Card.raceNmb);

            if (raceTypePossessed.Count == 0) {
                raceTypePossessed.Add(raceType);
                AlreadyAdded = true;
            }

            if (!AlreadyAdded) {
                for (int i = 0; i < raceTypePossessed.Count; i++) {
                    if (raceTypePossessed[i] < raceType) {
                        if (i == raceTypePossessed.Count - 1) {
                            raceTypePossessed.Add(raceType);
                            break;
                        }
                        continue;
                    }
                    else if (raceTypePossessed[i] == raceType) {
                        break;
                    }
                    raceTypePossessed.Insert(i, raceType);
                    break;
                }
            }
        }

        card.customTransform.parent = transform;

        if (cardTypesPossessed.Contains(card.cardType)) {
            int index = cardTypesPossessed.FindIndex(x => x == card.cardType);
            cardPositions[index].Add(new Vector3());
            cards[index].Add(card);
        }
        else {
            if (cardTypesPossessed.Count == 0) {
                cardTypesPossessed.Add(card.cardType);s
                cardPositions.Add(new List<Vector3>());
                cardPositions[0].Add(new Vector3());
                cards.Add(new List<Card>());
                cards[0].Add(card);
                return; 
            }

            for (int i = 0; i < cardTypesPossessed.Count; i++) {
                if (cardTypesPossessed[i] < card.cardType) {
                    if (i == cardTypesPossessed.Count - 1) {
                        cardTypesPossessed.Add(card.cardType);
                        cardPositions.Add(new List<Vector3>());
                        cardPositions[cardPositions.Count - 1].Add(new Vector3());
                        cards.Add(new List<Card>());
                        cards[cards.Count - 1].Add(card);
                        break;
                    }
                    continue;
                }

                cardTypesPossessed.Insert(i, card.cardType);
                cardPositions.Insert(i, new List<Vector3>());
                cardPositions[i].Add(new Vector3());
                cards.Insert(i, new List<Card>());
                cards[i].Add(card);
                break;
            }
        }

        for (int i = 0; i < cardPositions.Count; i++) {
            int index = raceTypePossessed.FindIndex(x => x == (CardManager.CardType)(cardTypesPossessed[i] - Card.raceNmb < 0 ? cardTypesPossessed[i] : cardTypesPossessed[i] - 10));
            for (int j = 0; j < cardPositions[i].Count; j++) {
                cardPositions[i][j] =
                    new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.1f,
                        cardInitialPos - Card.cardLength * j * decalBetweenCard
                        );
            }
        }

        for (CardManager.CardType i = CardManager.CardType.HUMAN_SECRET; i < CardManager.CardType.Length; i++) {
            if (!cardTypesPossessed.Contains(i))
                continue;

            int index = raceTypePossessed.FindIndex(x => x == i - Card.raceNmb);
            cardPositions[cardTypesPossessed.FindIndex(x => x == i)][0] =
                new Vector3(
                    (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                    0.0f,
                    cardInitialPos + Card.cardLength * secretCardAdvance

                    );
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

        for (int i = 0; i < raceTypePossessed.Count; i++) {
            Gizmos.color = new Color(
                1 - (float)raceTypePossessed[i] / Card.raceNmb,
                0,
                (float)raceTypePossessed[i] / Card.raceNmb,
                1
                );

            Gizmos.DrawCube(
                    new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * i - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.1f,
                        cardInitialPos + Card.cardLength * secretCardAdvance
                        ),
                    Vector3.one * 0.5f
                );
        }
    }
}
