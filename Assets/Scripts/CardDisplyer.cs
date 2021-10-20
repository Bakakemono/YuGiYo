using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CardDisplyer : MonoBehaviour
{
    float cardNumber = 0;
    Player player;
    List<List<Vector3>> positions;
    List<CardManager.CardType> positionsType;
    Vector2Int latestCardAdded = new Vector2Int(-1, -1);
    CardManager.CardType overviewedCardType = CardManager.CardType.NONE;
    CardManager.CardType selectedCardType = CardManager.CardType.NONE;
    [SerializeField] List<Card> cards = new List<Card>();
    const int TYPE_NOT_AVAILABLE = -1;


    [SerializeField] float maxAngle;
    [SerializeField] float fanningMaxAngle;

    [SerializeField] Vector3 fanPosition = Vector3.zero;
    [SerializeField] float circleRadius = 10.0f;
    public Vector3 circlePosition;

    List<Vector3> cardPositions = new List<Vector3>();

    const int INVALID_CARD = -1;
    int currentOverviewedCard = INVALID_CARD;
    int selectedCard = INVALID_CARD;
    int drawedCard = INVALID_CARD;
    float overviewDistanceCard = 0.5f;
    float cardMovingSpeed = 0.2f;
    Vector3 selectedCardSpotPosition = new Vector3(-3.0f, 2.0f);

    float acceptableSpaceLerp = 0.01f;

    public float maxAngleRadiant;
    public float fanningMaxAngleRadiant;

    [SerializeField] bool isPlayerTurn = false;

    DiscardPileManager discardPileManager;

    bool caseA = true;

    void Start() {
        player = GetComponent<Player>();
        positions = new List<List<Vector3>>();
        positionsType = new List<CardManager.CardType>();
        //cardNumber = cards.Count;
        foreach (Card card in cards) {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        discardPileManager = FindObjectOfType<DiscardPileManager>();

        //anglePerCard = fanningMaxAngleRadiant / (cardNumber - 1) < maxAngleRadiant ?
        //                    fanningMaxAngleRadiant / (cardNumber - 1) : maxAngleRadiant;

        //positions = new List<List<Vector3>>();
        //positionsType = new List<CardManager.CardType>();
        //for (int i = 0; i < hand.cards.Count; i++) {
        //    positions.Add(new List<Vector3>());
        //}
        //for (int i = 0; i < hand.cards.Count; i++) {
        //    for (CardManager.CardType j = 0; j < CardManager.CardType.Length; j++) {
        //        if (hand.cards.ContainsKey(j)) {
        //            foreach (Card card in hand.cards[j]) {
        //                positions[i].Add(new Vector3());
        //            }
        //            positionsType.Add(j);
        //            break;
        //        }
        //    }
        //}
    }

    private void FixedUpdate() {
        if (!isPlayerTurn)
            return;
        
        DrawCard();

        SelectACard();

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

        float anglePerCard = fanningMaxAngleRadiant / (positionsType.Count - 1) < maxAngleRadiant ?
                                fanningMaxAngleRadiant / (positionsType.Count - 1) : maxAngleRadiant;

        for (int i = 0; i < positions.Count; i++) {
            for (int j = 0; j < positions[i].Count; j++) {
                positions[i][j] = new Vector3(
                        circlePosition.x +
                            circleRadius *
                                (Mathf.Sin((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f - anglePerCard / 20.0f * j)),
                        circlePosition.y +
                            circleRadius *
                                (Mathf.Cos((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f - anglePerCard / 20.0f * j)),
                        circlePosition.z +
                            j * 0.005f
                                );
            }
        }

        for (int i = 0; i < positionsType.Count; i++) {
            for (int j = 0; j < positions[i].Count; j++) {
                player.hand.cards[positionsType[i]][j].customTransform.localPosition = positions[i][j];
                player.hand.cards[positionsType[i]][j].customTransform.localRotation = i == selectedCard ? Quaternion.identity :
                    Quaternion.Euler(new Vector3(0.0f, -10.0f, -((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f) * Mathf.Rad2Deg));
            }
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
            if (cardSelected != null && player.hand.IsCardInHand(cardSelected)) {
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
            if(!(cardSelected != null && player.hand.IsCardInHand(cardSelected))) {
                    return;
            }

            if(cardSelected == player.hand.cards[selectedCardType][0] && Input.GetMouseButtonUp(0)) {
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

                cardNumber++;
                AddCardToDisplay(newCard, player.hand.AddCard(newCard));
            }
        }
    }

    void AddCardToDisplay(Card card, Vector2Int newCardIndex) {
        card.customTransform.parent = transform;
        latestCardAdded = newCardIndex;

        if (positionsType.Contains((CardManager.CardType)latestCardAdded.x)) {
            Debug.Log("OldType");
            int index = FindIndex((CardManager.CardType)latestCardAdded.x);
            Debug.Log("Old type finded : " + index);
            positions[index].Add(new Vector3());
        }
        else {
            Debug.Log("New Type");
            if(positionsType.Count == 0) {
                positionsType.Add((CardManager.CardType)latestCardAdded.x);
                positions.Add(new List<Vector3>());
                positions[0].Add(new Vector3());
            }

            for (int i = 0; i < positionsType.Count; i++) {
                if ((int)positionsType[i] < latestCardAdded.x) {
                    if(i == positionsType.Count - 1) {
                        positionsType.Add((CardManager.CardType)latestCardAdded.x);
                        positions.Add(new List<Vector3>());
                        positions[positions.Count - 1].Add(new Vector3());
                        return;
                    }
                    continue;
                }

                positionsType.Insert(i, (CardManager.CardType)latestCardAdded.x);
                positions.Insert(i, new List<Vector3>());
                positions[i].Add(new Vector3());
                break;
            }
        }
    }

    void RemoveCardFromDisplay(Card card) {
        int index = FindIndex(card.cardType);
        positions[index].RemoveAt(0);
        if(positions[index].Count == 0) {
            positions.RemoveAt(index);
            positionsType.RemoveAt(index);
        }
    }

    int FindIndex(CardManager.CardType cardType) {
        int index = TYPE_NOT_AVAILABLE;
        index = positionsType.FindIndex(x => x == cardType);
        return index;
    }

    void SortCards() {
        cards.Sort((c1, c2) => c1.cardType.CompareTo(c2.cardType));
    }

    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.red;

    //    maxAngleRadiant = maxAngle / 360.0f * 2 * Mathf.PI;
    //    circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);



    //    Gizmos.DrawSphere(transform.position + circlePosition, 0.3f);

    //    Gizmos.DrawLine(
    //        transform.position + circlePosition,
    //            transform.position + new Vector3(
    //                circlePosition.x + circleRadius * Mathf.Sin(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
    //                circlePosition.y + circleRadius * Mathf.Cos(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
    //        );

    //    Gizmos.DrawLine(
    //        transform.position + circlePosition,
    //            transform.position + new Vector3(
    //                circlePosition.x + circleRadius * Mathf.Sin(fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
    //                circlePosition.y + circleRadius * Mathf.Cos(fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
    //       );
    //}
}
