using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class HandDisplayer : MonoBehaviour
{
    float cardNumber = 0;
    Player player;
    List<List<Vector3>> positions;
    List<CardManager.CardType> positionsType;
    Vector2Int latestCardAdded = new Vector2Int(-1, -1);
    CardManager.CardType overviewedCardType = CardManager.CardType.NONE;
    CardManager.CardType selectedCardType = CardManager.CardType.NONE;
    const int TYPE_NOT_AVAILABLE = -1;

    float anglePerCard = 0;

    [SerializeField] float maxAngle = 10.0f;
    [SerializeField] float fanningMaxAngle = 25.0f;

    [SerializeField] Vector3 fanPosition = Vector3.zero;
    [SerializeField] float circleRadius = 10.0f;
    public Vector3 circlePosition;

    List<Vector3> cardPositions = new List<Vector3>();

    const int INVALID_CARD = -1;
    int currentOverviewedCard = INVALID_CARD;
    int selectedCard = INVALID_CARD;
    int drawedCard = INVALID_CARD;
    float overviewElevationHeight = 0.5f;
    float overviewClosingDistance = 0.03f;
    float lerpSpeed = 0.2f;
    Vector3 selectedCardSpotPosition = new Vector3(-3.0f, 2.0f);

    float acceptableSpaceLerp = 0.01f;

    public float maxAngleRadiant;
    public float fanningMaxAngleRadiant;

    DiscardPileManager discardPileManager;

    bool caseA = true;

    void Start() {
        positions = new List<List<Vector3>>();
        positionsType = new List<CardManager.CardType>();

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        discardPileManager = FindObjectOfType<DiscardPileManager>();
    }

    private void FixedUpdate() {

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        //cardNumber = cards.Count;

        //float anglePerCard = fanningMaxAngleRadiant / (cardNumber - 1) < maxAngleRadiant ?
        //                    fanningMaxAngleRadiant / (cardNumber - 1) : maxAngleRadiant;

        ////// OLD START
        //for (int i = 0; i < cardNumber; i++) {

        //    cardPositions[i] = new Vector3(
        //        circlePosition.x +
        //            (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) *
        //                Mathf.Sin((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f),
        //        circlePosition.y +
        //            (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) *
        //                Mathf.Cos((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f));

        //}

        //if (selectedCard != INVALID_CARD)
        //    cardPositions[selectedCard] = fanPosition + selectedCardSpotPosition;


        //for (int i = 0; i < cardNumber; i++) {
        //    cards[i].customTransform.localPosition = Vector3.Lerp(cards[i].customTransform.localPosition, cardPositions[i], cardMovingSpeed);
        //    cards[i].customTransform.localRotation = i == selectedCard ? Quaternion.identity :
        //        Quaternion.Euler(new Vector3(0.0f, -10.0f, -((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f) * Mathf.Rad2Deg));
        //}

        //if (selectedCard != INVALID_CARD && (cards[selectedCard].customTransform.localPosition - (fanPosition + selectedCardSpotPosition)).sqrMagnitude < acceptableSpaceLerp * acceptableSpaceLerp) {
        //    PlayCard();
        //}

        ////// OLD END
        //return;
        // NEW START


        // Calculate the angle depending on how much card there are left and the space allocated.
        anglePerCard = fanningMaxAngleRadiant / (positionsType.Count - 1) < maxAngleRadiant ?
                                fanningMaxAngleRadiant / (positionsType.Count - 1) : maxAngleRadiant;

        // Calculate the position of each card around the fan
        for (int i = 0; i < positions.Count; i++) {
            for (int j = 0; j < positions[i].Count; j++) {
                positions[i][j] = new Vector3(
                        circlePosition.x +
                            circleRadius *
                                (Mathf.Sin((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f - anglePerCard / 20.0f * j)),
                        circlePosition.y +
                            circleRadius *
                                (Mathf.Cos((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f - anglePerCard / 20.0f * j)),
                        circlePosition.z +
                            j * 0.01f
                                );
            }
        }

        // Create an index variable for the selected card
        int index = INVALID_CARD;

        // If there is an overviewed card, Modifie the pos of the said card to expose it
        if (overviewedCardType != CardManager.CardType.NONE) {
            index = FindIndex(overviewedCardType);

            positions[index][0] = new Vector3(
                circlePosition.x +
                    (circleRadius + overviewElevationHeight) *
                        (Mathf.Sin((anglePerCard * index) - anglePerCard * (positions.Count - 1.0f) / 2.0f)),
                circlePosition.y +
                    (circleRadius + overviewElevationHeight) *
                        (Mathf.Cos((anglePerCard * index) - anglePerCard * (positions.Count - 1.0f) / 2.0f)),
                circlePosition.z +
                    -overviewClosingDistance
                        );
        }

        // Set the selected on the side to be viewed more in detail
        if (selectedCardType != CardManager.CardType.NONE) {
            index = FindIndex(selectedCardType);

            positions[index][0] = fanPosition + selectedCardSpotPosition;
        }
        else {
            index = INVALID_CARD;
        }

        //Apply the previously calcuclate position to each card
        for (int i = 0; i < positionsType.Count; i++) {
            float angle = -((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f) * Mathf.Rad2Deg;
            for (int j = 0; j < positions[i].Count; j++) {
                player.hand.cards[positionsType[i]][j].customTransform.localPosition =
                    Vector3.Lerp(player.hand.cards[positionsType[i]][j].customTransform.localPosition,
                    positions[i][j],
                    lerpSpeed);
                player.hand.cards[positionsType[i]][j].customTransform.localRotation =
                    Quaternion.Lerp(
                        player.hand.cards[positionsType[i]][j].customTransform.localRotation,
                        (index == i && j == 0) ? Quaternion.identity : Quaternion.Euler(new Vector3(0.0f, -10.0f, angle)),
                        lerpSpeed
                        );
            }
        }

        if (!player.canPlay)
            return;

        DrawCard();

        SelectACard();

        if (selectedCardType == CardManager.CardType.NONE &&
            index != INVALID_CARD &&
            (player.hand.cards[selectedCardType][0].customTransform.localPosition - (fanPosition + selectedCardSpotPosition)).sqrMagnitude < acceptableSpaceLerp * acceptableSpaceLerp) {
            PlayCard();
        }
    }

    void SelectACard() {
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit)) {
        //    Card cardSelected = hit.transform.GetComponent<Card>();

        //    currentOverviewedCard = cards.FindIndex(x => x == cardSelected);
        //}
        //else {
        //    currentOverviewedCard = INVALID_CARD;
        //}

        //if (currentOverviewedCard != INVALID_CARD && currentOverviewedCard != selectedCard &&
        //    Input.GetMouseButtonUp(0)) {
        //    selectedCard = currentOverviewedCard;
        //}
        //else if (currentOverviewedCard == INVALID_CARD && Input.GetMouseButtonUp(0)) {
        //    selectedCard = INVALID_CARD;
        //}
        //return;

        RaycastHit hitBis;
        Ray rayBis = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayBis, out hitBis)) {
            Card cardSelected = hitBis.transform.GetComponent<Card>();
            if (cardSelected != null && player.hand.IsCardInHand(cardSelected) && cardSelected.cardType != selectedCardType) {
                overviewedCardType = cardSelected.cardType;
            }
            else {
                overviewedCardType = CardManager.CardType.NONE;
            }
        }
        else {
            overviewedCardType = CardManager.CardType.NONE;
        }

        if (overviewedCardType != CardManager.CardType.NONE && overviewedCardType != selectedCardType &&
            Input.GetMouseButtonUp(0)) {
            selectedCardType = overviewedCardType;
        }
        else if (overviewedCardType == CardManager.CardType.NONE && Input.GetMouseButtonUp(0)) {
            selectedCardType = CardManager.CardType.NONE;
        }
    }

    void PlayCard() {
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Physics.Raycast(ray, out hit)) {
        //    Card cardSelected = hit.transform.GetComponent<Card>();

        //    currentOverviewedCard = cards.FindIndex(x => x == cardSelected);

        //    if(currentOverviewedCard == selectedCard && Input.GetMouseButtonUp(0)) {
        //        cardPositions.RemoveAt(selectedCard);
        //        cards.RemoveAt(currentOverviewedCard);
        //        selectedCard = INVALID_CARD;
        //        discardPileManager.DiscardCard(cardSelected);
        //        SortCards();

        //    }
        //}
        //return;

        RaycastHit hitBis;
        Ray rayBis = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayBis, out hitBis)) {
            Card cardSelected = hitBis.transform.GetComponent<Card>();
            if (!(cardSelected != null && player.hand.IsCardInHand(cardSelected))) {
                return;
            }

            if (cardSelected == player.hand.cards[selectedCardType][0] && Input.GetMouseButtonUp(0)) {
                player.hand.RemoveCard(cardSelected);
                RemoveCardFromDisplay(cardSelected);
                selectedCardType = CardManager.CardType.NONE;
                discardPileManager.DiscardCard(cardSelected);
            }
        }
    }

    void DrawCard() {
        if (caseA ? Input.GetKeyDown(KeyCode.D) : Input.GetKeyDown(KeyCode.F)) {
            if (caseA)
                caseA = false;
            else
                caseA = true;

            DrawPileManager drawPileManager = FindObjectOfType<DrawPileManager>();
            if (drawPileManager) {
                Card newCard = drawPileManager.DrawCard();
                if (newCard.cardType == CardManager.CardType.NONE)
                    return;
                player.hand.AddCard(newCard);
                AddCardToDisplay(newCard);
            }
        }
    }

    public void AddCardToDisplay(Card card) {
        card.customTransform.parent = transform;

        if (positionsType.Contains(card.cardType)) {
            int index = FindIndex(card.cardType);
            positions[index].Add(new Vector3());
        }
        else {
            if (positionsType.Count == 0) {
                positionsType.Add(card.cardType);
                positions.Add(new List<Vector3>());
                positions[0].Add(new Vector3());
                return;
            }

            for (int i = 0; i < positionsType.Count; i++) {
                if ((int)positionsType[i] < latestCardAdded.x) {
                    if (i == positionsType.Count - 1) {
                        positionsType.Add(card.cardType);
                        positions.Add(new List<Vector3>());
                        positions[positions.Count - 1].Add(new Vector3());
                        return;
                    }
                    continue;
                }

                positionsType.Insert(i, card.cardType);
                positions.Insert(i, new List<Vector3>());
                positions[i].Add(new Vector3());
                return;
            }
        }
    }

    public void RemoveCardFromDisplay(Card card) {
        int index = FindIndex(card.cardType);
        positions[index].RemoveAt(0);
        if (positions[index].Count == 0) {
            positions.RemoveAt(index);
            positionsType.RemoveAt(index);
        }
    }

    int FindIndex(CardManager.CardType cardType) {
        int index = TYPE_NOT_AVAILABLE;
        index = positionsType.FindIndex(x => x == cardType);
        return index;
    }

    public void SetPlayer(Player pl) {
        player = pl;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        Gizmos.DrawSphere(transform.position + circlePosition, 0.3f);

        Gizmos.DrawLine(
            transform.position + circlePosition,
                transform.position + new Vector3(
                    circlePosition.x + circleRadius * Mathf.Sin(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
                    circlePosition.y + circleRadius * Mathf.Cos(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
            );

        Gizmos.DrawLine(
            transform.position + circlePosition,
                transform.position + new Vector3(
                    circlePosition.x + circleRadius * Mathf.Sin(fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
                    circlePosition.y + circleRadius * Mathf.Cos(fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
           );

        //Gizmos.color = Color.red;
        //Debug.Log("Number of type registered : " + positions.Count);
        //for (int i = 0; i < positions.Count; i++) {
        //    Gizmos.DrawCube(transform.TransformPoint(new Vector3(
        //                circlePosition.x +
        //                    circleRadius *
        //                        (Mathf.Sin((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f)),
        //                circlePosition.y +
        //                    circleRadius *
        //                        (Mathf.Cos((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f))
        //                        )),
        //                        Vector3.one * 0.5f
        //                        );
        //}
    }
}
