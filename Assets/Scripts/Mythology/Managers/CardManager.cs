using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviourPunCallbacks {
    public enum CardType : int {
        HUMAN,
        WEREWOLF,
        OGRE,
        DRYAD,
        KORRIGAN,
        GNOME,
        FAIRY,
        LEPRECHAUN,
        FARFADET,
        ELF,
        HUMAN_SECRET,
        WEREWOLF_SECRET,
        OGRE_SECRET,
        DRYAD_SECRET,
        KORRIGAN_SECRET,
        GNOME_SECRET,
        FAIRY_SECRET,
        LEPRECHAUN_SECRET,
        FARFADET_SECRET,
        ELF_SECRET,
        BEER,
        Length,
        TOTAL_RACE_NUMBER = HUMAN_SECRET,
        NONE
    }

    [SerializeField]
    public List<int> cardsNumber = new List<int> {
        16,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        12
    };

    public static int totalRaceCount = 10;

    [SerializeField] CardEffect[] cardEffects;

    public enum CardEndLocaion {
        FIELD,
        DISCARD_PILE
    }

    public enum ProgressCardPlayed {
        DRAW_FIRST_CARD,
        SELECT_CARD_TO_PLAY,
        SELECT_TARGET,
        SELECT_TARGET_HAND_CARD,
        SELECT_TARGET_FIELD_CARD,
        SELECT_HAND_CARD,
        SELECT_FIELD_CARD,
        UPDATE_ALL_PLAYER,
        ANIMATION_PLAYIING,
        CARD_SHOWCASE,
        EFFECT,
        CARD_TO_FINAL_LOCATION,
        END_OF_TURN
    }

    ProgressCardPlayed progressCardPlayed = ProgressCardPlayed.SELECT_CARD_TO_PLAY;


    [SerializeField] GameObject card;

    Dictionary<CardType, List<Card>> cards;

    [SerializeField] List<Material> cardMaterials = new List<Material>();

    // Components
    PhotonView view;

    // Other Managers
    GameManager gameManager;
    DrawPileManager drawPileManager;
    DiscardPileManager discardPileManager;

    // List of all players
    [SerializeField] public Player[] players;

    // Number of card that each player will get at the start of the 
    [SerializeField] int startingCardNumber = 6;

    bool startDistributingFirstHand = false;

    public bool initialHandGiven = false;

    [SerializeField] Transform cardShowcasePosition;

    readonly int moveFrameCountTotal = 50;
    int moveFrameCountCurrent = 0;

    Vector3 initialPos;
    Quaternion initialRotation;

    int totalCards;

    // Data from player playing a card
    Card cardPlayed = null;
    Player player = null;

    // Data result from player selection
    Player target = null;
    List<Vector2> targetHandCards = new List<Vector2>();
    List<Vector2> targetFieldCards = new List<Vector2>();
    List<Vector2> handCards = new List<Vector2>();
    List<Vector2> fieldCards = new List<Vector2>();

    bool targetSelected = false;
    bool targetHandCardSelected = false;
    bool targetFieldCardSelected = false;

    bool doSelectHand = false;
    bool doSelectField = false;

    bool effectNullified = false;

    void Awake() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
        discardPileManager = FindObjectOfType<DiscardPileManager>();
        gameManager = FindObjectOfType<GameManager>();
        view = GetComponent<PhotonView>();

        cards = new Dictionary<CardType, List<Card>>();

        cardShowcasePosition = FindObjectOfType<SlotDistributor>().GetShowCasePosition();
    }

    private void FixedUpdate() {

        // Once the first hand is given, the main game loop start
        if(initialHandGiven) {
            CardPlayedProgress();
        }
    }

    #region Draw Pile Creation
    // Instantiate all the card that will be played during the game.
    public void InstantiateCards() {
        List<GameObject> allCards = new List<GameObject>();

        for(int i = 0; i < cardsNumber.Count; i++) {
            cards.Add((CardType)i, new List<Card>());
            for(int j = 0; j < cardsNumber[i]; j++) {
                allCards.Add(Instantiate(card, Vector3.one * 1000.0f, Quaternion.identity));
                Card newCard = allCards[allCards.Count - 1].GetComponent<Card>();
                newCard.SetCardType((CardType)i);
                newCard.SetId(j);
                newCard.SetCardEffect(cardEffects[i]);
                newCard.UpdateCard(cardMaterials[i]);

                cards[(CardType)i].Add(newCard);
            }
        }
        totalCards = allCards.Count;

        if(PhotonNetwork.IsMasterClient) {
            PrepareCardsOrder();
        }
    }

    // The master client shuffle the cards and then send the order to each players.
    void PrepareCardsOrder() {
        List<CardType> allTypes = new List<CardType>();
        int[] cardsOrder = new int[totalCards];
        List<int> cardsCount = cardsNumber;

        for(CardType type = 0; type != CardType.Length; type++) {
            allTypes.Add(type);
        }

        int index = 0;

        while(true) {
            int typeSelected = Random.Range(0, allTypes.Count);

            if(cardsCount[typeSelected] <= 0) {
                cardsCount.RemoveAt(typeSelected);
                allTypes.RemoveAt(typeSelected);
                if(allTypes.Count == 0) {
                    break;
                }
            }
            else {
                cardsOrder[index] = (int)allTypes[typeSelected];

                cardsCount[typeSelected]--;
                index++;
            }
        }

        view.RPC("RPC_InitializeDrawPile", RpcTarget.All, cardsOrder);
    }

    // Use the order recieved from the master clien and each player will setup their card order
    // and then send the info to the Draw Pile Manager.
    [PunRPC]
    void RPC_InitializeDrawPile(int[] _cardsOrder) {
        Dictionary<CardType, List<Card>> copyCards = new Dictionary<CardType, List<Card>>();
        for(int i = 0; i < cards.Keys.Count; i++) {
            copyCards.Add((CardType)i, new List<Card>(cards[(CardType)i]));
        }
        List<Card> drawPileCards = new List<Card>();
        int cardsCountdown = totalCards;

        int index = 0;

        while(true) {
            drawPileCards.Insert(0, copyCards[(CardType)_cardsOrder[index]][0]);
            drawPileCards[0].GetTransform().localPosition =
                new Vector3(Random.Range(-100.0f, 100.0f), 1000.0f, Random.Range(-100.0f, 100.0f));
            drawPileCards[0].GetTransform().parent = drawPileManager.transform;
            copyCards[(CardType)_cardsOrder[index]].RemoveAt(0);

            index++;
            cardsCountdown--;
            if(cardsCountdown == 0)
                break;
        }

        drawPileManager.InitializeDrawPile(drawPileCards);
    }
    #endregion

    // Give the designated player a certain number of card.
    public void DrawCardToPlayer(Player player, int numberOfCard, float timeToWait) {
        if(numberOfCard == 1) {
            Vector2Int index = player.GetHand().AddCard(drawPileManager.DrawCard());
            player.handManager.AddCard(player.GetHand().cards[(CardType)index.x][index.y]);
        }
        else {
            StartCoroutine(DrawMultipleCard(player, numberOfCard, timeToWait));
        }
    }

    // Give multiple card 
    IEnumerator DrawMultipleCard(Player player, int numberOfCard, float timeToWait) {
        for(int i = 0; i < numberOfCard; i++) {
            Vector2Int index = player.GetHand().AddCard(drawPileManager.DrawCard());
            player.handManager.AddCard(player.GetHand().cards[(CardType)index.x][index.y]);
            yield return new WaitForSeconds(timeToWait);
        }
    }

    // Give the cards each player should have at the begining of the game.
    IEnumerator DrawInitialHand() {
        for(int i = 0; i < startingCardNumber; i++) {
            for(int j = 0; j < players.Length; j++) {
                DrawCardToPlayer(players[(gameManager.playerTurn + j) % GameManager.EXPECTED_PLAYER_NUMBER], 1, 0);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return new WaitForSeconds(4.0f);
        initialHandGiven = true;
    }

    // Return where the card should go when played depending on the type
    public CardEndLocaion CardLocationGoal(CardType cardType) {
        switch(cardType) {
            case CardType.BEER:
                return CardEndLocaion.DISCARD_PILE;

            default:
                return CardEndLocaion.FIELD;
        }
    }

    // Register the played card
    public void PlayCard(Player _player, Card _cardPlayed) {
        player = _player;
        cardPlayed = _cardPlayed;
    }

    [PunRPC]
    void RPC_UpdatePlayers(
            Vector2 _cardPlayed,
            int _targetSelected,
            Vector2[] _targetHandCards,
            Vector2[] _targetFieldCards,
            Vector2[] _handCards,
            Vector2[] _fieldCards,
            bool _effectNullified
            ) {
        player = players[gameManager.playerTurn];
        cardPlayed = cards[(CardType)_cardPlayed.x][(int)_cardPlayed.y];
        if(_targetSelected != GameManager.NO_ID) {
            target = players[gameManager.ConvertPlayerIndex(_targetSelected)];
        }

        targetHandCards = new List<Vector2>(_targetHandCards);
        targetFieldCards = new List<Vector2>(_targetFieldCards);
        handCards = new List<Vector2>(_handCards);
        fieldCards = new List<Vector2>(_fieldCards);

        player.PlayCard(cardPlayed);

        moveFrameCountCurrent = 0;
        cardPlayed.GetTransform().parent = cardShowcasePosition.transform;
        initialRotation = cardPlayed.GetTransform().localRotation;
        initialPos = cardPlayed.GetTransform().localPosition;

        effectNullified = _effectNullified;

        progressCardPlayed = ProgressCardPlayed.CARD_SHOWCASE;
    }

    public void CardPlayedProgress() {
        switch(progressCardPlayed) {
            case ProgressCardPlayed.DRAW_FIRST_CARD:
                break;

            case ProgressCardPlayed.SELECT_CARD_TO_PLAY:
                if(player != null && cardPlayed != null) {
                    if(player.id == gameManager.GetOwnerId()) {
                        moveFrameCountCurrent = 0;
                        cardPlayed.GetTransform().parent = cardShowcasePosition.transform;
                        initialRotation = cardPlayed.GetTransform().localRotation;
                        initialPos = cardPlayed.GetTransform().localPosition;

                        if(IsEffectNotPlayable(cardPlayed.GetCardEffect())) {
                            effectNullified = true;
                            progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                        }

                        if(cardPlayed.GetCardEffect().DoTargetPlayer()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_TARGET;
                            break;
                        }
                        else if(cardPlayed.GetCardEffect().DoSelectCard()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                            break;
                        }
                        else if(cardPlayed.GetCardEffect().DoSelectField()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                            break;
                        }
                        progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                    }
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET:
                if(cardPlayed.GetCardEffect().DoTargetHand()) {
                    doSelectHand = true;
                }
                else if(cardPlayed.GetCardEffect().DoTargetField()) {
                    doSelectField = true;
                }
                if(targetSelected) {
                    doSelectHand = false;
                    doSelectField = false;
                    targetSelected = false;
                    if(cardPlayed.GetCardEffect().DoTargetHand()) {
                        //gameManager.SetCameraToPlayer(target.id, true);
                        progressCardPlayed = ProgressCardPlayed.SELECT_TARGET_HAND_CARD;
                        break;
                    }
                    else if(cardPlayed.GetCardEffect().DoTargetField()) {
                        gameManager.SetCameraToPlayer(target.id, false);
                        progressCardPlayed = ProgressCardPlayed.SELECT_TARGET_FIELD_CARD;
                        break;
                    }
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET_HAND_CARD:
                target.SetTargetHand(true);
                if(targetHandCardSelected) {
                    target.SetTargetHand(false);
                    targetHandCardSelected = false;

                    gameManager.SetCameraToPlayer(player.id);

                    if(cardPlayed.GetCardEffect().DoSelectCard()) {
                        progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                        break;
                    }
                    else if(cardPlayed.GetCardEffect().DoSelectField()) {
                        progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                        break;
                    }
                    else {
                        progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                        break;
                    }
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET_FIELD_CARD:
                target.SetTargetField(true);
                if(targetFieldCardSelected) {
                    target.SetTargetField(false);
                    targetFieldCardSelected = false;

                    gameManager.SetCameraToPlayer(player.id);

                    if(cardPlayed.GetCardEffect().DoSelectCard()) {
                        progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                        break;
                    }
                    else if(cardPlayed.GetCardEffect().DoSelectField()) {
                        progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                        break;
                    }
                    else {
                        progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                        break;
                    }
                }
                break;

            case ProgressCardPlayed.SELECT_HAND_CARD:
                progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                break;

            case ProgressCardPlayed.SELECT_FIELD_CARD:
                progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                break;

            case ProgressCardPlayed.UPDATE_ALL_PLAYER:
                int targetId = target == null ? GameManager.NO_ID : target.id;
                view.RPC(
                    "RPC_UpdatePlayers",
                    RpcTarget.Others,
                    new Vector2((float)cardPlayed.GetCardType(), cardPlayed.GetId()),
                    targetId,
                    targetHandCards.ToArray(),
                    targetFieldCards.ToArray(),
                    handCards.ToArray(),
                    fieldCards.ToArray(),
                    effectNullified
                    );
                progressCardPlayed = ProgressCardPlayed.CARD_SHOWCASE;
                break;

            case ProgressCardPlayed.CARD_SHOWCASE:
                player.canPlay = false;
                moveFrameCountCurrent++;
                cardPlayed.GetTransform().localPosition = Vector3.Lerp(initialPos, Vector3.zero, (float)moveFrameCountCurrent / moveFrameCountTotal);
                cardPlayed.GetTransform().localRotation = Quaternion.Lerp(initialRotation, Quaternion.identity, (float)moveFrameCountCurrent / moveFrameCountTotal);
                if(moveFrameCountCurrent >= moveFrameCountTotal) {
                    if(effectNullified) {
                        progressCardPlayed = ProgressCardPlayed.CARD_TO_FINAL_LOCATION;
                        break;
                    }
                    progressCardPlayed = ProgressCardPlayed.EFFECT;
                }
                break;

            case ProgressCardPlayed.EFFECT:
                cardPlayed.GetCardEffect().Effect(
                    player,
                    target,
                    cardPlayed.GetCardEffect().DoTargetHand() ? targetHandCards.ToArray() : targetFieldCards.ToArray(),
                    cardPlayed.GetCardEffect().DoTargetHand() ? handCards.ToArray() : fieldCards.ToArray(),
                    this
                    );
                progressCardPlayed = ProgressCardPlayed.CARD_TO_FINAL_LOCATION;
                break;

            case ProgressCardPlayed.CARD_TO_FINAL_LOCATION:
                if(CardLocationGoal(cardPlayed.GetCardType()) == CardEndLocaion.DISCARD_PILE && player.GetHand().cards.Count > 0) {
                    player.GetHand().RemoveCard(cardPlayed);
                    discardPileManager.DiscardCard(cardPlayed);
                    player.canPlay = true;
                    progressCardPlayed = ProgressCardPlayed.END_OF_TURN;
                }
                else {
                    player.GetHand().RemoveCard(cardPlayed);
                    player.GetField().AddCard(cardPlayed);
                    player.fieldManager.AddCard(cardPlayed);
                    player.canPlay = true;
                    gameManager.NextPlayer();
                    progressCardPlayed = ProgressCardPlayed.END_OF_TURN;
                }
                break;

            case ProgressCardPlayed.END_OF_TURN:
                player = null;
                cardPlayed = null;
                effectNullified = false;
                progressCardPlayed = ProgressCardPlayed.SELECT_CARD_TO_PLAY;
                break;
        }
    }

    public Card GetCard(Vector2 _cardInfo) {
        return cards[(CardType)_cardInfo.x][(int)_cardInfo.y];
    }
    public Card GetCard(CardType cardType, int id) {
        return cards[cardType][id];
    }

    #region Transfert Cards
    void TransferCardsOwnership(Player _source, Player _receiver, bool _isHandSource, bool _isHandReceiver, Vector2[] _cards) {
        StartCoroutine(TransferCardsOwnerShipRoutine(_source, _receiver, _isHandSource, _isHandReceiver, _cards));
    }

    IEnumerator TransferCardsOwnerShipRoutine(Player _source, Player _receiver, bool _isHandSource, bool _isHandReceiver, Vector2[] _cards) {
        for(int i = 0; i < _cards.Length; i++) {
            Card targetedCard = cards[(CardType)_cards[i].x][((int)_cards[i].y)];
            if(_isHandSource) {
                _source.RemoveCardFromHand(targetedCard);
            }
            else {
                _source.RemoveCardFromField(targetedCard);
            }

            if(_isHandReceiver) {
                _receiver.AddCardtoHand(targetedCard);
            }
            else {
                _receiver.AddCardtoField(targetedCard);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Discard Cards
    void SendCardToDiscard(Player _source, bool _isHandSource, Vector2[] _cards) {
        StartCoroutine(SendCardToDiscardRoutine(_source, _isHandSource, _cards));
    }

    IEnumerator SendCardToDiscardRoutine(Player _source, bool _isHandSource, Vector2[] _cards) {
        for(int i = 0; i < _cards.Length; i++) {
            Card targetedCard = cards[(CardType)_cards[i].x][((int)_cards[i].y)];
            if(_isHandSource) {
                _source.RemoveCardFromHand(targetedCard);
            }
            else {
                _source.RemoveCardFromField(targetedCard);
            }

            discardPileManager.DiscardCard(targetedCard);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    // Copy the player array into the card manager
    public void RegisterPlayers(Player[] gmPlayers) {
        players = gmPlayers;
    }

    public void StartGivingInitialHand() {
        StartCoroutine(DrawInitialHand());
    }


    // For Selecter
    public ProgressCardPlayed GetProgressCardPlayed() {
        return progressCardPlayed;
    }

    // Register the targeted player
    public void SelectTarget(int localTargetId) {
        target = players[localTargetId];
        targetSelected = true;
    }

    // Register the targeted card in hand
    public bool SelectTargetHandCard(Vector2 cardInfo) {
        if(!targetHandCards.Contains(cardInfo)) {
            targetHandCards.Add(cardInfo);
            return true;
        }
        return false;
    }

    // Register the targeted or type on field
    public bool SelectTargetFieldCard(List<Vector2> _cardsInfos) {
        for(int i = 0; i < _cardsInfos.Count; i++) {
            if(!targetFieldCards.Contains(_cardsInfos[i])) {
                return false;
            }
            targetFieldCards.Add(_cardsInfos[i]);

        }
        return true;
    }

    public bool GetDoSelectHand() {
        return doSelectHand;
    }

    public bool GetDoSelectField() {
        return doSelectField;
    }

    private bool IsEffectNotPlayable(CardEffect cardEffect) {
        return (cardPlayed.GetCardEffect().DoTargetHand() ?
                            players[1].GetHand().IsEmpty() &&
                            players[2].GetHand().IsEmpty() &&
                            players[3].GetHand().IsEmpty() :
                            false) ||
                (cardPlayed.GetCardEffect().DoTargetField() ?
                            players[1].GetHand().IsEmpty() &&
                            players[2].GetHand().IsEmpty() &&
                            players[3].GetHand().IsEmpty() :
                            false) ||
                (cardPlayed.GetCardEffect().DoSelectCard() ?
                            players[0].GetHand().IsEmpty() :
                            false) ||
                (cardPlayed.GetCardEffect().DoSelectField() ?
                            players[0].GetHand().IsEmpty() :
                            false);
    }
}