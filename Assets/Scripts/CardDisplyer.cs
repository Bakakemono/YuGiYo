using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CardDisplyer : MonoBehaviour
{
    float cardNumber;
    [SerializeField] List<Transform> cards = new List<Transform>();
    [SerializeField] float maxAngle;
    [SerializeField] float fanningMaxAngle;

    [SerializeField] Vector3 fanPosition = Vector3.zero;
    [SerializeField] float circleRadius = 10.0f;
    public Vector3 circlePosition;

    List<Vector3> cardPositions = new List<Vector3>();

    const int INVALID_CARD = -1;
    int currentOverviewedCard = INVALID_CARD;
    int selectedCard = INVALID_CARD;
    float overviewDistanceCard = 0.5f;
    float cardMovingSpeed = 0.2f;
    Vector3 selectedCardSpotPosition = new Vector3(0, 2.0f);

    public float maxAngleRadiant;
    public float fanningMaxAngleRadiant;

    void Start() {
        cardNumber = cards.Count;
        foreach (Transform card in cards) {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;
    }

    private void FixedUpdate() {
        SelectACard();

        cardNumber = cards.Count;
        cardPositions.Clear();
        foreach (Transform card in cards)
        {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        float anglePerCard = fanningMaxAngleRadiant / (cardNumber - 1) < maxAngleRadiant ? fanningMaxAngleRadiant / (cardNumber - 1) : maxAngleRadiant;
         
        for(int i = 0; i < cardNumber; i++) {
            

            cardPositions[i] = new Vector3(
                circlePosition.x + (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) * Mathf.Sin((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f),
                circlePosition.y + (i == currentOverviewedCard ? circleRadius + overviewDistanceCard : circleRadius) * Mathf.Cos((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f));
        }

        for (int i = 0; i < cardNumber; i++) {
            if(i == selectedCard)
                continue;

            cards[i].localPosition = Vector3.Lerp(cards[i].localPosition, cardPositions[i], cardMovingSpeed);
            cards[i].localRotation = 
                Quaternion.Euler(new Vector3(0.0f, -10.0f, -((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f) * Mathf.Rad2Deg));
        }
        if(selectedCard == INVALID_CARD)
            return;
        cards[selectedCard].localPosition = Vector3.Lerp(cards[selectedCard].localPosition, fanPosition + selectedCardSpotPosition, cardMovingSpeed);
        cards[selectedCard].localRotation = Quaternion.Lerp(cards[selectedCard].localRotation, Quaternion.identity, cardMovingSpeed);
    }

    void SelectACard() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Transform cardSelected = hit.transform;

            currentOverviewedCard = cards.FindIndex(x => x == cardSelected);
        } else {
            currentOverviewedCard = -1;
        }

        if (currentOverviewedCard != INVALID_CARD && currentOverviewedCard != selectedCard &&
            Input.GetMouseButtonUp(0)) {
            selectedCard = currentOverviewedCard;
        }
        else if (currentOverviewedCard == INVALID_CARD && Input.GetMouseButtonUp(0)) {
            selectedCard = INVALID_CARD;
        }
    }

    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.red;

    //    maxAngleRadiant = maxAngle / 360.0f * 2 * Mathf.PI;
    //    circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);


    //    Gizmos.DrawSphere(circlePosition, 0.3f);

    //    Gizmos.DrawLine(
    //        circlePosition,
    //            new Vector3(
    //                circlePosition.x + circleRadius * Mathf.Sin(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
    //                circlePosition.y + circleRadius * Mathf.Cos(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
    //        );

    //    Gizmos.DrawLine(
    //        circlePosition,
    //            new Vector3(
    //                circlePosition.x + circleRadius * Mathf.Sin(fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
    //                circlePosition.y + circleRadius * Mathf.Cos(fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
    //       );
    //}
}
