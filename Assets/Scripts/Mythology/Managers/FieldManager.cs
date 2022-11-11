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
    float secretCardAdvance = 1.2f;

    readonly float lerpSpeed = 0.2f;

    CardManager.CardType overviewedCardType = CardManager.CardType.NONE;
    readonly float overviewedHeight = 0.2f;

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

    bool addCards = true;

    void Start() {
        cardPositions = new List<List<Vector3>>();
        cardTypesPossessed = new List<CardManager.CardType>();
        raceTypePossessed = new List<CardManager.CardType>();

        cardInitialPos = Card.cardLength / 2 + Card.cardLength * decalBetweenCard * cardMaxNumberPerColumn;
    }
    private void Update() {
        if(player != null && player.IsFieldTargeted()) {
            SelectCard();
        }
        else {
            overviewedCardType = CardManager.CardType.NONE;
        }
    }

    private void FixedUpdate() {
        CardManager.CardType currentType = CardManager.CardType.HUMAN;
        for (int i = 0; i < cardPositions.Count; i++) {
            if (!player.GetField().cards.ContainsKey(currentType)) {
                while (true) {
                    currentType++;
                    if (player.GetField().cards.ContainsKey(currentType))
                        break; 
                }
            }

            if(overviewedCardType == cardTypesPossessed[i]) {
                for(int j = 0; j < cardPositions[i].Count - selectedCardTypesNumbers[i]; j++) {
                    player.GetField().cards[currentType][j].GetTransform().localPosition =
                        Vector3.Lerp(
                            player.GetField().cards[currentType][j].GetTransform().localPosition,
                            cardPositions[i][j] + Vector3.up * overviewedHeight,
                            lerpSpeed);

                    player.GetField().cards[currentType][j].GetTransform().localRotation =
                        Quaternion.Lerp(
                            player.GetField().cards[currentType][j].GetTransform().localRotation,
                            Quaternion.Euler(89.0f, 0.0f, 0.0f),
                            lerpSpeed);
                }
            }
            else {
                for(int j = 0; j < cardPositions[i].Count - selectedCardTypesNumbers[i]; j++) {
                    player.GetField().cards[currentType][j].GetTransform().localPosition =
                        Vector3.Lerp(
                            player.GetField().cards[currentType][j].GetTransform().localPosition,
                            cardPositions[i][j],
                            lerpSpeed);

                    player.GetField().cards[currentType][j].GetTransform().localRotation =
                        Quaternion.Lerp(    
                            player.GetField().cards[currentType][j].GetTransform().localRotation,
                            Quaternion.Euler(89.0f, 0.0f, 0.0f),
                            lerpSpeed);
                }
            }
            for(int j = cardPositions[i].Count - selectedCardTypesNumbers[i]; j < cardPositions[i].Count; j++) {
                player.GetField().cards[currentType][j].GetTransform().localPosition =
                    Vector3.Lerp(
                        player.GetField().cards[currentType][j].GetTransform().localPosition,
                        cardPositions[i][j] + Vector3.up * selectedCardheight,
                        lerpSpeed);

                player.GetField().cards[currentType][j].GetTransform().localRotation =
                    Quaternion.Lerp(
                        player.GetField().cards[currentType][j].GetTransform().localRotation,
                        Quaternion.Euler(89.0f, 0.0f, 0.0f),
                        lerpSpeed);
            }

            currentType++;
        }
    }

    public void AddCard(Card _card) {
        if (!cardTypesPossessed.Contains(_card.GetCardType() - CardManager.totalRaceCount) && !cardTypesPossessed.Contains(_card.GetCardType())) {
            CardManager.CardType raceType = (_card.GetCardType() - CardManager.totalRaceCount < 0 ? _card.GetCardType() : _card.GetCardType() - CardManager.totalRaceCount);

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

        _card.GetTransform().parent = transform;

        if (cardTypesPossessed.Contains(_card.GetCardType())) {
            int index = cardTypesPossessed.FindIndex(x => x == _card.GetCardType());
            cardPositions[index].Add(new Vector3());
        }
        else {
            if (cardTypesPossessed.Count == 0) {
                cardTypesPossessed.Add(_card.GetCardType());
                cardPositions.Add(new List<Vector3>());
                cardPositions[0].Add(new Vector3());

                selectedCardTypesNumbers.Add(0);
            }
            else {
                for (int i = 0; i < cardTypesPossessed.Count; i++) {
                    if (cardTypesPossessed[i] < _card.GetCardType()) {
                        if (i == cardTypesPossessed.Count - 1) {
                            cardTypesPossessed.Add(_card.GetCardType());
                            cardPositions.Add(new List<Vector3>());
                            
                            // Add a new position at the last elements
                            cardPositions[^1].Add(new Vector3());

                            selectedCardTypesNumbers.Add(0);
                            break;
                        }
                        continue;
                    }

                    cardTypesPossessed.Insert(i, _card.GetCardType());
                    cardPositions.Insert(i, new List<Vector3>());
                    cardPositions[i].Add(new Vector3());

                    selectedCardTypesNumbers.Insert(i, 0);
                    break;
                }
            }
        }

        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardTypesPossessed[i] > (CardManager.CardType)CardManager.totalRaceCount)
                break;

            int index = raceTypePossessed.FindIndex(x => x == (cardTypesPossessed[i] - CardManager.totalRaceCount < 0 ? cardTypesPossessed[i] : cardTypesPossessed[i] - 10));
            for (int j = 0; j < cardPositions[i].Count; j++) {
                cardPositions[i][j] =
                    new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.02f,
                        Card.cardLength * 0.5f + Card.cardLength * (cardPositions[i].Count - j - 1) * decalBetweenCard
                        );
            }
        }

        for (CardManager.CardType i = CardManager.CardType.HUMAN_SECRET; i < CardManager.CardType.Length; i++) {
            if (!cardTypesPossessed.Contains(i))
                continue;

            int index = raceTypePossessed.FindIndex(x => x == i - (int)CardManager.CardType.TOTAL_PEOPLE_NUMBER);
            cardPositions[cardTypesPossessed.FindIndex(x => x == i)][0] =
                new Vector3(
                    (Card.cardWidth + spaceBetweenColumn) * index - (raceTypePossessed.Count - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                    0.0f,
                    secretCardAdvance * Card.cardLength +
                        (cardTypesPossessed.Contains(i - (int)CardManager.CardType.TOTAL_PEOPLE_NUMBER) ? cardPositions[index][0].z : 0)
                    );
        }
    }

    public void RemoveCard(Card _card) {
        int index = cardTypesPossessed.FindIndex(x => x == _card.GetCardType());
        cardPositions[index].RemoveAt(0);
        if (cardPositions[index].Count == 0) {
            cardPositions.RemoveAt(index);
            cardTypesPossessed.RemoveAt(index);

            selectedCardTypesNumbers.RemoveAt(index);

            if (!cardTypesPossessed.Contains(_card.GetCardType() + CardManager.totalRaceCount) && !cardTypesPossessed.Contains(_card.GetCardType() - CardManager.totalRaceCount)) {
                raceTypePossessed.Remove(_card.GetCardType() - CardManager.totalRaceCount < 0 ? _card.GetCardType() : _card.GetCardType() - CardManager.totalRaceCount);
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

            if(cardSelected != null && 
                player.GetField().IsCardOnField(cardSelected) &&
                cardSelected.GetCardType() < CardManager.CardType.TOTAL_PEOPLE_NUMBER) {
                overviewedCardType = cardSelected.GetCardType();
            }
            else {
                overviewedCardType = CardManager.CardType.NONE;
            }
        }
        else {
            overviewedCardType = CardManager.CardType.NONE;
        }

        // Select the card that you click on if there is one currently overviewed.
        if(overviewedCardType != CardManager.CardType.NONE && Input.GetMouseButtonUp(0)) {
            int typeIndex = raceTypePossessed.FindIndex(x => x == overviewedCardType);
            if(!player.IsTypeTargeted() && cardPositions[typeIndex].Count > selectedCardTypesNumbers[typeIndex]) {
                selectedCardTypes.Add(overviewedCardType);
                selectedCardTypesNumbers[typeIndex]++;
                selectedcards.Add(
                    new Vector2(
                        (float)overviewedCardType,
                        // Get the ID of the last element minus the number of card selected
                        player.GetField().cards[overviewedCardType][^(selectedCardTypesNumbers[typeIndex])].GetId()
                        )
                    );
            }
            else if(player.IsTypeTargeted() && !selectedCardTypes.Contains(overviewedCardType)) {
                selectedCardTypes.Add(overviewedCardType);
                selectedCardTypesNumbers[typeIndex] = player.GetField().cards[overviewedCardType].Count;
                for(int i = 0; i < selectedCardTypesNumbers[typeIndex]; i++) {
                    selectedcards.Add(
                        new Vector2(
                            (float)overviewedCardType,
                            player.GetField().cards[overviewedCardType][i].GetId()
                            )
                        );
                }
            }
            if(player.GetCardNumberToTarget() >= selectedCardTypes.Count || selectedCardTypes.Count >= player.GetField().GetTotalCardNumber()) {
                player.TargetFieldCard(new List<Vector2>(selectedcards));
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

    public void ResetFieldManager() {
        cardPositions.Clear();
        cardTypesPossessed.Clear();
        raceTypePossessed.Clear();

        overviewedCardType = CardManager.CardType.NONE;

        numberCardsToSelect = 0;
        selectedCardTypes.Clear();
        selectedCardTypesNumbers.Clear();

        selectedcards.Clear();
    }

    private void OnDrawGizmosSelected() {
        int raceNumber = 10;

        cardInitialPos = Card.cardLength / 2 + Card.cardLength * decalBetweenCard * cardMaxNumberPerColumn;

        float left = -(raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f - Card.cardWidth / 2.0f;
        float right = (Card.cardWidth + spaceBetweenColumn) * (raceNumber - 1) - (raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f + Card.cardWidth / 2.0f;
        float top = Card.cardLength + Card.cardLength * (raceNumber - 1) * decalBetweenCard;
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

        for(int i = 0; i < cardMaxNumberPerColumn; i++) {
            Gizmos.DrawLine(
                transform.TransformPoint(new Vector3(right, 0, Card.cardLength + Card.cardLength * (cardMaxNumberPerColumn - i - 1) * decalBetweenCard)),
                transform.TransformPoint(new Vector3(left, 0, Card.cardLength + Card.cardLength * (cardMaxNumberPerColumn - i - 1) * decalBetweenCard))
                );
        }

        for(int i = 0; i < raceNumber; i++) {
            for(int j = 0; j < cardMaxNumberPerColumn; j++) {
                Gizmos.DrawSphere(transform.TransformPoint(new Vector3(
                        (Card.cardWidth + spaceBetweenColumn) * i - (raceNumber - 1) * (Card.cardWidth + spaceBetweenColumn) / 2.0f,
                        0.1f,
                        Card.cardLength * 0.5f + Card.cardLength * (cardMaxNumberPerColumn - j - 1) * decalBetweenCard
                        )),
                        0.02f
                        );
                ;
            }
        }
    }
}