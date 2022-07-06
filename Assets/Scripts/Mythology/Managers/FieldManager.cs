using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviourPunCallbacks {
    [SerializeField] Player player;
    List<List<Vector3>> cardPositions;

    // All type of cards currently displayed by the field manager, Race and Secret Differentiated.
    List<CardManager.CardType> cardTypesPossessed;

    // Only the race are included in this one.
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

    CardManager.CardType overviewedCardType = CardManager.CardType.NONE;
    float overviewedHeight = 0.2f;

    [SerializeField] int numberCardsToSelect = 0;
    [SerializeField] List<CardManager.CardType> selectedCardTypes = new List<CardManager.CardType>();
    [SerializeField] List<int> selectedCardTypesNumbers = new List<int>();
    [SerializeField] float selectedCardheight = 0.3f;

    [SerializeField] List<Vector2> selectedcards = new List<Vector2>();

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

        cardInitialPos = Card.cardLength / 2 + Card.cardLength * decalBetweenCard * cardMaxNumberPerColumn;
    }
    private void Update() {
        if(player != null && player.doTargetField) {
            SelectCard();
        }
        else {
            overviewedCardType = CardManager.CardType.NONE;
        }
    }

    private void FixedUpdate() {
        CardManager.CardType currentType = CardManager.CardType.HUMAN;
        for (int i = 0; i < cardPositions.Count; i++) {
            if (!player.field.cards.ContainsKey(currentType)) {
                while (true) {
                    currentType++;
                    if (player.field.cards.ContainsKey(currentType))
                        break; 
                }
            }
            if(player.doTargetType && !selectedCardTypes.Contains(overviewedCardType) && overviewedCardType == cardTypesPossessed[i]) {
                
            }

            if(overviewedCardType == cardTypesPossessed[i]) {
                for(int j = 0; j < cardPositions[i].Count - selectedCardTypesNumbers[i]; j++) {
                    player.field.cards[currentType][j].customTransform.localPosition =
                        Vector3.Lerp(
                            player.field.cards[currentType][j].customTransform.localPosition,
                            cardPositions[i][j] + Vector3.up * overviewedHeight,
                            lerpSpeed);

                    player.field.cards[currentType][j].customTransform.localRotation =
                        Quaternion.Lerp(
                            player.field.cards[currentType][j].customTransform.localRotation,
                            Quaternion.Euler(89.0f, 0.0f, 0.0f),
                            lerpSpeed);
                }
            }
            else {
                for(int j = 0; j < cardPositions[i].Count - selectedCardTypesNumbers[i]; j++) {
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
            }
            for(int j = cardPositions[i].Count - selectedCardTypesNumbers[i]; j < cardPositions[i].Count; j++) {
                player.field.cards[currentType][j].customTransform.localPosition =
                    Vector3.Lerp(
                        player.field.cards[currentType][j].customTransform.localPosition,
                        cardPositions[i][j] + Vector3.up * selectedCardheight,
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

                selectedCardTypesNumbers.Add(0);
            }
            else {
                for (int i = 0; i < cardTypesPossessed.Count; i++) {
                    if (cardTypesPossessed[i] < card.cardType) {
                        if (i == cardTypesPossessed.Count - 1) {
                            cardTypesPossessed.Add(card.cardType);
                            cardPositions.Add(new List<Vector3>());
                            cardPositions[cardPositions.Count - 1].Add(new Vector3());

                            selectedCardTypesNumbers.Add(0);
                            break;
                        }
                        continue;
                    }

                    cardTypesPossessed.Insert(i, card.cardType);
                    cardPositions.Insert(i, new List<Vector3>());
                    cardPositions[i].Add(new Vector3());

                    selectedCardTypesNumbers.Insert(i, 0);
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
                        0.02f,
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

    public void RemoveCardFormDisplay(Card card) {
        int index = cardTypesPossessed.FindIndex(x => x == card.cardType);
        cardPositions[index].RemoveAt(0);
        if (cardPositions[index].Count == 0) {
            cardPositions.RemoveAt(index);
            cardTypesPossessed.RemoveAt(index);

            selectedCardTypesNumbers.RemoveAt(index);

            if (!cardTypesPossessed.Contains(card.cardType + Card.raceNmb) && !cardTypesPossessed.Contains(card.cardType - Card.raceNmb)) {
                raceTypePossessed.Remove(card.cardType - Card.raceNmb < 0 ? card.cardType : card.cardType - Card.raceNmb);
            }
        }
    }

    // Select cards and put them on overviewed mode.
    void SelectCard() {
        RaycastHit hitBis;
        Ray rayBis = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Put a card currently under the mouse on overviewed mode if she is in hand.
        if(Physics.Raycast(rayBis, out hitBis)) {
            Card cardSelected = hitBis.transform.GetComponent<Card>();
            if(cardSelected != null && player.field.IsCardOnField(cardSelected)) {
                overviewedCardType = cardSelected.cardType;
            }
            else {
                overviewedCardType = CardManager.CardType.NONE;
            }
        }
        else {
            overviewedCardType = CardManager.CardType.NONE;
        }

        // Select the card that you click on if there is one currently overviewed.
        if(overviewedCardType != CardManager.CardType.NONE &&
            Input.GetMouseButtonUp(0)) {
            int typeIndex = raceTypePossessed.FindIndex(x => x == overviewedCardType);
            if(!player.doTargetType && cardPositions[typeIndex].Count > selectedCardTypesNumbers[typeIndex]) {
                selectedCardTypes.Add(overviewedCardType);
                selectedCardTypesNumbers[typeIndex]++;
                selectedcards.Add(
                    new Vector2(
                        (float)overviewedCardType,
                        player.field.cards[overviewedCardType]
                            [player.field.cards[overviewedCardType].Count - (selectedCardTypesNumbers[typeIndex])].id
                        )
                    );
            }
            else if(player.doTargetType && !selectedCardTypes.Contains(overviewedCardType)) {
                selectedCardTypes.Add(overviewedCardType);
                selectedCardTypesNumbers[typeIndex] = player.hand.cards[overviewedCardType].Count;
                for(int i = 0; i < selectedCardTypesNumbers[typeIndex]; i++) {
                    selectedcards.Add(
                        new Vector2(
                            (float)overviewedCardType,
                            player.field.cards[overviewedCardType][i].id
                            )
                        );
                }
            }
            if(numberCardsToSelect == selectedCardTypes.Count) {
                // TODO : the parametter type to a list so that every cards can be transmitted
                player.TargetFieldCard(new Vector2((float)overviewedCardType, player.field.cards[overviewedCardType][0].id));
                new List<Vector2>(selectedcards);
                selectedCardTypes.Clear();
                for(int i = 0; i < selectedCardTypesNumbers.Count; i++) {
                    selectedCardTypesNumbers[i] = 0;
                }
                selectedcards.Clear();
            }
        }
    }

    public void SetNumberCardToSelect(int number) {
        numberCardsToSelect = number;
    }


    public void SetPlayer(Player _player) {
        player = _player;
    }

    private void OnDrawGizmosSelected() {
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
                        0.02f
                        );
                ;
            }
        }
    }
}