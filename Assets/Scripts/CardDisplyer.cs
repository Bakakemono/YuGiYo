using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CardDisplyer : MonoBehaviour
{
    float cardNumber;
    [SerializeField] List<Card> cards = new List<Card>();
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

    void Start() {
        cardNumber = cards.Count;
        foreach (Card card in cards) {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        discardPileManager = FindObjectOfType<DiscardPileManager>();
    }

    private void FixedUpdate() {
        if(Input.GetMouseButtonUp(0) && isPlayerTurn)
            DrawCard();

        SelectACard();

        cardNumber = cards.Count;
        cardPositions.Clear();
        foreach (Card card in cards)
        {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        float anglePerCard = fanningMaxAngleRadiant / (cardNumber - 1) < maxAngleRadiant ?
                                    fanningMaxAngleRadiant / (cardNumber - 1) : maxAngleRadiant;
         
        for(int i = 0; i < cardNumber; i++) {
            cardPositions[i] = new Vector3(
                circlePosition.x + 
                    (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) *
                        Mathf.Sin((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f),
                circlePosition.y + 
                    (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) *
                        Mathf.Cos((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f));
        }

        if(selectedCard != INVALID_CARD)
            cardPositions[selectedCard] = fanPosition + selectedCardSpotPosition;


        for (int i = 0; i < cardNumber; i++) {
            cards[i].customTransform.localPosition = Vector3.Lerp(cards[i].customTransform.localPosition, cardPositions[i], cardMovingSpeed);
            cards[i].customTransform.localRotation = i == selectedCard ? Quaternion.identity :
                Quaternion.Euler(new Vector3(0.0f, -10.0f, -((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f) * Mathf.Rad2Deg));
        }

        if(selectedCard != INVALID_CARD && (cards[selectedCard].customTransform.localPosition - (fanPosition + selectedCardSpotPosition)).sqrMagnitude < acceptableSpaceLerp * acceptableSpaceLerp) {
            PlayCard();
        }
    }

    void SelectACard() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Card cardSelected = hit.transform.GetComponent<Card>();

            currentOverviewedCard = cards.FindIndex(x => x == cardSelected);
        }
        else {
            currentOverviewedCard = INVALID_CARD;
        }

        if (currentOverviewedCard != INVALID_CARD && currentOverviewedCard != selectedCard &&
            Input.GetMouseButtonUp(0)) {
            selectedCard = currentOverviewedCard;
        }
        else if (currentOverviewedCard == INVALID_CARD && Input.GetMouseButtonUp(0)) {
            selectedCard = INVALID_CARD;
        }
    }

    void PlayCard() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            Card cardSelected = hit.transform.GetComponent<Card>();
            
            currentOverviewedCard = cards.FindIndex(x => x == cardSelected);

            if(currentOverviewedCard == selectedCard && Input.GetMouseButtonUp(0)) {
                cards.RemoveAt(currentOverviewedCard);
                selectedCard = INVALID_CARD;
                discardPileManager.DiscardCard(cardSelected);
                SortCards();
            }
        }
    }

    void DrawCard() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            DrawPileManager drawPileManager = hit.transform.GetComponent<DrawPileManager>();
            if (drawPileManager) {
                Card newCard = drawPileManager.DrawCard();
                newCard.customTransform.parent = transform;
                cards.Add(newCard);
                SortCards();
                drawedCard = cards.FindIndex(x => x == newCard);
            }
        }
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
