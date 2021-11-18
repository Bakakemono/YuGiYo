using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldDisplayer : MonoBehaviour {
    [SerializeField] Player player;
    List<List<Vector3>> cardPositions;
    List<CardManager.CardType> cardTypesPossessed;
    [SerializeField] List<CardManager.CardType> raceTypePossessed;

    float cardInitialPos = 5.0f;

    int cardMaxNumberPerColumn = 10;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float spaceBetweenColumn = 0.05f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float decalBetweenCard = 0.2f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float secretCardAdvance = 0.75f;

    float lerpSpeed = 0.2f;

    int frameMovementTotal = 50;
    int frameCountCurrent = 0;

    Vector2Int lastCardAdded = new Vector2Int();
    Vector2Int NO_CARD {
        get {
            return new Vector2Int(-1, -1);
        }
    }

    bool addCarts = true;
    void Start() {
        cardPositions = new List<List<Vector3>>();
        cardTypesPossessed = new List<CardManager.CardType>();
        raceTypePossessed = new List<CardManager.CardType>();
    }

    private void Update() {
        CardManager.CardType currentType = CardManager.CardType.HUMAN;
        for (int i = 0; i < cardPositions.Count; i++) {
            if (!player.field.cards.ContainsKey(currentType)) {
                while (true) {
                    currentType++;
                    if (player.field.cards.ContainsKey(currentType))
                        break;
                }
            }
            

            for (int j = 0; j < cardPositions[i].Count; j++) {                
                player.field.cards[currentType][j].customTransform.localPosition =
                    Vector3.Lerp(
                        player.field.cards[currentType][j].customTransform.localPosition,
                        cardPositions[i][j],
                        lerpSpeed);

                player.field.cards[currentType][j].customTransform.localRotation =
                    Quaternion.Lerp(
                        player.field.cards[currentType][j].customTransform.localRotation,
                        Quaternion.Euler(89.0f, 0.0f, 0.0f),
                        lerpSpeed);
            }
            currentType++;
        }
    }

    public void AddCardToField(Card card) {
        if (!cardTypesPossessed.Contains(card.cardType - Card.raceNmb) && !cardTypesPossessed.Contains(card.cardType)) {
            CardManager.CardType raceType = (card.cardType - Card.raceNmb < 0 ? card.cardType : card.cardType - Card.raceNmb);

            if (raceTypePossessed.Count == 0) {
                raceTypePossessed.Add(raceType);
            }
            else {
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
        }
        else {
            if (cardTypesPossessed.Count == 0) {
                cardTypesPossessed.Add(card.cardType);
                cardPositions.Add(new List<Vector3>());
                cardPositions[0].Add(new Vector3());
            }
            else {
                for (int i = 0; i < cardTypesPossessed.Count; i++) {
                    if (cardTypesPossessed[i] < card.cardType) {
                        if (i == cardTypesPossessed.Count - 1) {
                            cardTypesPossessed.Add(card.cardType);
                            cardPositions.Add(new List<Vector3>());
                            cardPositions[cardPositions.Count - 1].Add(new Vector3());
                            break;
                        }
                        continue;
                    }

                    cardTypesPossessed.Insert(i, card.cardType);
                    cardPositions.Insert(i, new List<Vector3>());
                    cardPositions[i].Add(new Vector3());
                    break;
                }
            }
        }

        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardTypesPossessed[i] > (CardManager.CardType)Card.raceNmb)
                break;

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

    void RemoveCardFormDisplay(Card card) {
        int index = cardTypesPossessed.FindIndex(x => x == card.cardType);
        cardPositions[index].RemoveAt(0);
        if (cardPositions[index].Count == 0) {
            cardPositions.RemoveAt(index);
            cardTypesPossessed.RemoveAt(index);
            if (!cardTypesPossessed.Contains(card.cardType + Card.raceNmb) && !cardTypesPossessed.Contains(card.cardType - Card.raceNmb))
                raceTypePossessed.Remove(card.cardType - Card.raceNmb < 0 ? card.cardType : card.cardType - Card.raceNmb);
        }
    }
}
