using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class HandDisplayer : MonoBehaviour
{
    Player player;
    bool playerSet = false;

    List<List<Vector3>> positions;
    List<CardManager.CardType> positionsType;

    // The type of the card currently overviewed and selected.
    [SerializeField] CardManager.CardType overviewedCardType = CardManager.CardType.NONE;
    [SerializeField] CardManager.CardType selectedCardType = CardManager.CardType.NONE;

    const int TYPE_NOT_AVAILABLE = -1;

    int selectedCardIndex;

    float anglePerCard = 0;

    // Angle at which each card are placed on the fan and the max angle for the fan to display card.
    [SerializeField] float maxAngle = 10.0f;
    [SerializeField] float fanningMaxAngle = 25.0f;

    // Angle value for the fan stored as Radiant instead of degree.
    float maxAngleRadiant;
    float fanningMaxAngleRadiant;


    [SerializeField] Vector3 fanPosition = Vector3.zero;
    [SerializeField] float circleRadius = 10.0f;
    public Vector3 circlePosition;

    const int INVALID_INDEX = -1;

    // Parameters of the placement for the card that is overviewed.
    float overviewElevationHeight = 0.5f;
    float overviewClosingDistance = 0.03f;

    // Spot where the card is placed when selected.
    Vector3 selectedCardSpotPosition = new Vector3(-3.0f, 2.0f);
    
    // Ratio at wich value lerped are closing at each time.
    float lerpValue = 0.2f;

    // Distance used to check if the values that are lerped are close enough to be consider equal.
    float acceptableSpaceLerp = 0.01f;

    void Start() {
        positions = new List<List<Vector3>>();
        positionsType = new List<CardManager.CardType>();

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;
    }

    private void Update() {
        if(!playerSet)
            return;

        SelectACard();

        if (!player.canPlay)
            return;

        if (selectedCardType != CardManager.CardType.NONE &&
            selectedCardIndex != INVALID_INDEX && 
            (player.hand.cards[selectedCardType][0].customTransform.localPosition - (fanPosition + selectedCardSpotPosition)).sqrMagnitude <
                acceptableSpaceLerp * acceptableSpaceLerp) {

            PlayCard();
        }
    }

    private void FixedUpdate() {
        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        // Calculate the angle depending on how much card there are left and the space allocated.
        anglePerCard = fanningMaxAngleRadiant / (positionsType.Count - 1) < maxAngleRadiant ?
                                fanningMaxAngleRadiant / (positionsType.Count - 1) : maxAngleRadiant;

        // Calculate the position of each card around the fan.
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

        // Create an index variable for the selected card.
        int overviewedCardIndex = INVALID_INDEX;

        // If there is an overviewed card, Modifie the pos of the said card to expose it.
        if (overviewedCardType != CardManager.CardType.NONE) {
            overviewedCardIndex = FindIndex(overviewedCardType);

            positions[overviewedCardIndex][0] = new Vector3(
                circlePosition.x +
                    (circleRadius + overviewElevationHeight) *
                        (Mathf.Sin((anglePerCard * overviewedCardIndex) - anglePerCard * (positions.Count - 1.0f) / 2.0f)),
                circlePosition.y +
                    (circleRadius + overviewElevationHeight) *
                        (Mathf.Cos((anglePerCard * overviewedCardIndex) - anglePerCard * (positions.Count - 1.0f) / 2.0f)),
                circlePosition.z +
                    -overviewClosingDistance
                        );
        }

        // Set the selected on the side to be viewed more in detail
        if (selectedCardType != CardManager.CardType.NONE) {
            selectedCardIndex = FindIndex(selectedCardType);

            positions[selectedCardIndex][0] = fanPosition + selectedCardSpotPosition;
        }
        else {
            selectedCardIndex = INVALID_INDEX;
        }

        //Apply the previously calcuclate position to each card
        for (int i = 0; i < positionsType.Count; i++) {
            float angle = -((anglePerCard * i) - anglePerCard * (positions.Count - 1.0f) / 2.0f) * Mathf.Rad2Deg;
            for (int j = 0; j < positions[i].Count; j++) {
                player.hand.cards[positionsType[i]][j].customTransform.localPosition =
                    Vector3.Lerp(player.hand.cards[positionsType[i]][j].customTransform.localPosition,
                    positions[i][j],
                    lerpValue);
                player.hand.cards[positionsType[i]][j].customTransform.localRotation =
                    Quaternion.Lerp(
                        player.hand.cards[positionsType[i]][j].customTransform.localRotation,
                        (selectedCardIndex == i && j == 0) ? Quaternion.identity : Quaternion.Euler(new Vector3(0.0f, -10.0f, angle)),
                        lerpValue
                        );
            }
        }
    }

    // Select cards and put them on overviewed mode or selected mode.
    void SelectACard() {
        RaycastHit hitBis;
        Ray rayBis = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Put a card currently under the mouse on overviewed mode if she is in hand.
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

        // Select the card that you click on if the overviewed type is not the same as
        // the current selected one and if there is one currently overviewed.
        if (overviewedCardType != CardManager.CardType.NONE && overviewedCardType != selectedCardType &&
            Input.GetMouseButtonUp(0)) {
            selectedCardType = overviewedCardType;
        }
        // If there is no type currently overviewed and thât the mouse is pressed cleare the current selected type.
        else if (overviewedCardType == CardManager.CardType.NONE && Input.GetMouseButtonUp(0)) {
            selectedCardType = CardManager.CardType.NONE;
        }
    }

    // Play the card selected by clicking on it
    void PlayCard() {
        RaycastHit hitBis;
        Ray rayBis = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayBis, out hitBis)) {
            Card cardSelected = hitBis.transform.GetComponent<Card>();
            // If there is no card overviewed or if there is the card is not in hand.
            if (!(cardSelected != null && player.hand.IsCardInHand(cardSelected))) {
                return;
            }

            // If the card currently overviewed is the card selected and the mouse is clicked play the card.
            if (cardSelected == player.hand.cards[selectedCardType][0] && Input.GetMouseButtonUp(0)) {
                player.hand.RemoveCard(cardSelected);
                RemoveCardFromDisplay(cardSelected);
                selectedCardType = CardManager.CardType.NONE;
                player.PlayCard(cardSelected);
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
                if (positionsType[i] < card.cardType) {
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
        playerSet = true;
    }

    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.red;

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
