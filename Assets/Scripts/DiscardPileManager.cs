using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileManager : MonoBehaviour
{
    List<Transform> cards = new List<Transform>();
    bool updateLastCard = false;
    float cardThickness = 0.01f;
    int lastCardIndex = -1;
    float acceptableSpace = 0.01f;
    float cardLerpSpeed = 0.2f;

    Vector3 lastCardPosition;


    public void DiscardCard(Transform card) {
        cards.Add(card);
        card.parent = transform;
        lastCardIndex = cards.Count - 1;
        lastCardPosition = new Vector3(0, 0, -cardThickness / 2.0f + lastCardIndex * cardThickness);
        updateLastCard = true;
    }

    private void FixedUpdate() {
        if (!updateLastCard)
            return;

        cards[lastCardIndex].localPosition = 
            Vector3.Lerp(
                cards[lastCardIndex].localPosition,
                lastCardPosition,
                cardLerpSpeed
                );

        cards[lastCardIndex].localRotation =
            Quaternion.Lerp(
                cards[lastCardIndex].localRotation,
                Quaternion.identity,
                cardLerpSpeed
                );

        if ((cards[lastCardIndex].localPosition - lastCardPosition).sqrMagnitude < acceptableSpace * acceptableSpace) {
            cards[lastCardIndex].localPosition = lastCardPosition;
            cards[lastCardIndex].localRotation = Quaternion.identity;
            updateLastCard = false;
        }
    }
}
