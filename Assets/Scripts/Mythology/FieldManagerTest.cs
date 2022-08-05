using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManagerTest : MonoBehaviour {
    [SerializeField] List<List<Vector3>> cardPositions = new List<List<Vector3>>();
    [SerializeField] List<CardManager.CardType> cardTypesPossessed = new List<CardManager.CardType>();
    [SerializeField] List<CardManager.CardType> raceTypePossessed = new List<CardManager.CardType>();

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

    [SerializeField] bool update = true;

    private void OnDrawGizmos() {
        if(!update)
            return;

        int raceNumber = 10;

        cardInitialPos = Card.cardLength / 2 + Card.cardLength * decalBetweenCard * cardMaxNumberPerColumn;

        float left = -(raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f - Card.cardWidth / 2.0f;
        float right = (Card.cardWidth + spaceBetweenColumn) * (raceNumber - 1) - (raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f + Card.cardWidth / 2.0f;
        float top = cardInitialPos + Card.cardLength / 2.0f;
        float bottom = 0;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.TransformPoint(new Vector3(left, 0, bottom)), transform.TransformPoint(new Vector3(right, 0, bottom)));
        Gizmos.DrawLine(transform.TransformPoint(new Vector3(left, 0, top)), transform.TransformPoint(new Vector3(right, 0, top)));
        Gizmos.DrawLine(transform.TransformPoint(new Vector3(left, 0, bottom)), transform.TransformPoint(new Vector3(left, 0, top)));
        Gizmos.DrawLine(transform.TransformPoint(new Vector3(right, 0, bottom)), transform.TransformPoint(new Vector3(right, 0, top)));

        for(int i = 0; i < raceNumber; i++) {
            float xPos = (Card.cardWidth + spaceBetweenColumn) * i - (raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f + Card.cardWidth / 2.0f;

            Gizmos.DrawLine(transform.TransformPoint(new Vector3(xPos, 0, bottom)), transform.TransformPoint(new Vector3(xPos, 0, top)));
        }

        for(int i = 0; i < raceNumber; i++) {
            for(int j = 0; j < cardMaxNumberPerColumn; j++) {
                Gizmos.DrawSphere(transform.TransformPoint(new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * i - (raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.1f,
                        cardInitialPos - Card.cardLength * j * decalBetweenCard
                        )),
                        0.1f
                        );
                ;
            }
        }

        // Logical part

        Field field = new Field();
        cardPositions = new List<List<Vector3>>();
        cardTypesPossessed = new List<CardManager.CardType>();
        raceTypePossessed = new List<CardManager.CardType>();

        cardInitialPos = Card.cardLength / 2 + Card.cardLength * decalBetweenCard * cardMaxNumberPerColumn;

        Card[] cards = GetComponentsInChildren<Card>();
        for(int i = 0; i < cards.Length; i++) {
            field.AddCard(cards[i]);
            
            if(!cardTypesPossessed.Contains(cards[i].GetCardType() - CardManager.totalRaceCount) && !cardTypesPossessed.Contains(cards[i].GetCardType())) {
                CardManager.CardType raceType = (cards[i].GetCardType() - CardManager.totalRaceCount < 0 ? cards[i].GetCardType() : cards[i].GetCardType() - CardManager.totalRaceCount);

                if(raceTypePossessed.Count == 0) {
                    raceTypePossessed.Add(raceType);
                }
                else {
                    for(int j = 0; j < raceTypePossessed.Count; j++) {
                        if(raceTypePossessed[j] < raceType) {
                            if(j == raceTypePossessed.Count - 1) {
                                raceTypePossessed.Add(raceType);
                                break;
                            }
                            continue;
                        }
                        else if(raceTypePossessed[j] == raceType) {
                            break;
                        }
                        raceTypePossessed.Insert(j, raceType);
                        break;
                    }
                }
            }

            cards[i].GetTransform().parent = transform;

            if(cardTypesPossessed.Contains(cards[i].GetCardType())) {
                int index = cardTypesPossessed.FindIndex(x => x == cards[i].GetCardType());
                cardPositions[index].Add(new Vector3());
            }
            else {
                if(cardTypesPossessed.Count == 0) {
                    cardTypesPossessed.Add(cards[i].GetCardType());
                    cardPositions.Add(new List<Vector3>());
                    cardPositions[0].Add(new Vector3());
                }
                else {
                    for(int j = 0; j < cardTypesPossessed.Count; j++) {
                        if(cardTypesPossessed[j] < cards[i].GetCardType()) {
                            if(i == cardTypesPossessed.Count - 1) {
                                cardTypesPossessed.Add(cards[i].GetCardType());
                                cardPositions.Add(new List<Vector3>());
                                cardPositions[cardPositions.Count - 1].Add(new Vector3());
                                break;
                            }
                            continue;
                        }

                        cardTypesPossessed.Insert(j, cards[i].GetCardType());
                        cardPositions.Insert(j, new List<Vector3>());
                        cardPositions[j].Add(new Vector3());
                        break;
                    }
                }
            }

            for(int j = 0; j < cardPositions.Count; j++) {
                if(cardTypesPossessed[j] > (CardManager.CardType)CardManager.totalRaceCount)
                    break;

                int index = raceTypePossessed.FindIndex(x => x == (CardManager.CardType)(cardTypesPossessed[j] - CardManager.totalRaceCount < 0 ? cardTypesPossessed[j] : cardTypesPossessed[j] - 10));
                for(int k = 0; k < cardPositions[j].Count; k++) {
                    cardPositions[j][k] =
                        new Vector3(
                            (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                            0.02f,
                            cardInitialPos - Card.cardLength * k * decalBetweenCard
                            );
                }
            }

            for(CardManager.CardType j = CardManager.CardType.HUMAN_SECRET; j < CardManager.CardType.Length; j++) {
                if(!cardTypesPossessed.Contains(j))
                    continue;

                int index = raceTypePossessed.FindIndex(x => x == j - CardManager.totalRaceCount);
                cardPositions[cardTypesPossessed.FindIndex(x => x == j)][0] =
                    new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.0f,
                        cardInitialPos + Card.cardLength * secretCardAdvance
                        );
            }
        }

        CardManager.CardType currentType = CardManager.CardType.HUMAN;
        for(int i = 0; i < cardPositions.Count; i++) {
            if(!field.cards.ContainsKey(currentType)) {
                while(true) {
                    currentType++;
                    if(field.cards.ContainsKey(currentType))
                        break;
                }
            }


            for(int j = 0; j < cardPositions[i].Count; j++) {
                field.cards[currentType][j].GetTransform().localPosition = cardPositions[i][j];
                field.cards[currentType][j].GetTransform().localRotation = Quaternion.Euler(89.0f, 0.0f, 0.0f);
            }
            currentType++;
        }
    }
}
