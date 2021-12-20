using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    List<Card> cards = new List<Card>();
    float cardThickness = 0.005f;
    Vector2 cardDimension = new Vector2(1.5f, 2.5f);

    BoxCollider boxCollider;

    float lerpSpeed = 0.2f;
    bool organising = false;
    public bool drawPileInstantiated = false;

    float cardSpeed = 2.0f;
    float timeBetweenCard = 0.02f;

    void Start() {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate() {
        if (!organising)
            return;

        OrganiseDeck();
    }

    void CalculateHitBoxSize() {
        boxCollider.size = new Vector3(cardDimension.x, cardDimension.y, (cardThickness * 3) * cards.Count);
        boxCollider.center = new Vector3(0, 0, (cardThickness * 3) * cards.Count / 2.0f);
    }

    public Card DrawCard() {
        if(cards.Count == 0) {
            return new Card();
        }
        Card DrawedCard = cards[0];
        cards.RemoveAt(0);
        CalculateHitBoxSize();
        return DrawedCard;
    }

    void OrganiseDeck() {
        for (int i = 0; i < cards.Count; i++) {
            cards[i].customTransform.localPosition = 
                Vector3.Lerp(
                    cards[i].customTransform.localPosition,
                    new Vector3(0, 0, cardThickness / 2.0f + (cards.Count - i) * (cardThickness)),
                    lerpSpeed * cardSpeed
                    );

            cards[i].customTransform.localRotation = 
                Quaternion.Lerp(
                    cards[i].customTransform.localRotation,
                    Quaternion.identity,
                    lerpSpeed * cardSpeed
                    );
        }
        CalculateHitBoxSize();
    }

    public void InitializeDrawPile(List<GameObject> allCards) {
        StartCoroutine(AddCardsToDrawPile(allCards));
    }

    public IEnumerator AddCardsToDrawPile(List<GameObject> allCards) {
        yield return new WaitForFixedUpdate();
        organising = true;

        bool shuffle = true;
        int totalCards = allCards.Count - 1;
        while (shuffle) {
            int cardSelected = Random.Range(0, totalCards);
            
            cards.Insert(0, allCards[cardSelected].GetComponent<Card>());
            cards[0].customTransform.localPosition = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(0.0f, 30.0f), Random.Range(-100.0f, 100.0f));
            allCards[cardSelected].transform.parent = transform;
            allCards.RemoveAt(cardSelected);

            totalCards--;
            if (totalCards == 0)
                shuffle = false;

            yield return new WaitForSeconds(timeBetweenCard);
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < cards.Count; i++) {
            cards[i].customTransform.localPosition = new Vector3(0, 0, cardThickness / 2.0f + (cards.Count - i) * (cardThickness));

            cards[i].customTransform.localRotation = Quaternion.identity;
        }

        organising = false;
        drawPileInstantiated = true;
    }
}
