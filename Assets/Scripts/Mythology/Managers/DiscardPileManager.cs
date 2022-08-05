using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileManager : MonoBehaviour
{
    List<Card> cards = new List<Card>();
    bool updateLastCard = false;
    float cardThickness = 0.001f;
    int lastCardIndex = -1;
    float acceptableSpace = 0.01f;
    float cardLerpSpeed = 0.2f;

    Vector3 lastCardPosition;


    public void DiscardCard(Card card) {
        cards.Add(card);
        card.GetTransform().parent = transform;
        lastCardIndex = cards.Count - 1;
        lastCardPosition = new Vector3(0, 0, -cardThickness / 2.0f + lastCardIndex * -cardThickness);
        updateLastCard = true;
    }

    private void FixedUpdate() {
        if (!updateLastCard)
            return;

        cards[lastCardIndex].GetTransform().localPosition = 
            Vector3.Lerp(
                cards[lastCardIndex].GetTransform().localPosition,
                lastCardPosition,
                cardLerpSpeed
                );

        cards[lastCardIndex].GetTransform().localRotation =
            Quaternion.Lerp(
                cards[lastCardIndex].GetTransform().localRotation,
                Quaternion.identity,
                cardLerpSpeed
                );

        if ((cards[lastCardIndex].GetTransform().localPosition - lastCardPosition).sqrMagnitude < acceptableSpace * acceptableSpace) {
            cards[lastCardIndex].GetTransform().localPosition = lastCardPosition;
            cards[lastCardIndex].GetTransform().localRotation = Quaternion.identity;
            updateLastCard = false;
        }
    }
}
