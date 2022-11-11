using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct CardsNumber {
    public int[] cardsNumber;
}

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
        TOTAL_PEOPLE_NUMBER = HUMAN_SECRET,
        NONE
    }

    [SerializeField]
    public List<int> cardNumberInDeck = new List<int> {};

    [SerializeField] string deckToUseName = "NumberOfCardsPerPeople";

    public static int totalRaceCount = 10;

    [SerializeField] CardEffect[] cardEffects;

    public enum CardEndLocaion {
        FIELD,
        DISCARD_PILE
    }

    public enum TurnProgress {
        DRAW_FIRST_CARD,
        SELECT_CARD_TO_PLAY,
        CARDS_SELECTION,
        UPDATE_ALL_PLAYER,
        ANIMATION_PLAYIING,
        CARD_SHOWCASE,
        EFFECT,
        CARD_TO_FINAL_LOCATION,
        END_OF_TURN
    }

    public enum CardSelectionStep {
        SELECT_TARGET,
        SELECT_TARGET_HAND_CARD,
        SELECT_TARGET_FIELD_CARD,
        SELECT_PLAYER_HAND_CARD,
        SELECT_PLAYER_FIELD_CARD
    }

    TurnProgress turnProgress = TurnProgress.SELECT_CARD_TO_PLAY;


    [SerializeField] GameObject cardPrefab;

    Dictionary<CardType, List<Card>> deck;

    [SerializeField] List<Material> cardMaterials = new List<Material>();

    // Components
    PhotonView view;

    // Other Managers
    GameManager gameManager;
    ShuffledDeckManager shuffledDeckManager;
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


    CardEffect currentCardEffect;
    List<CardSelectionStep> cardSelectionSteps;
    int cardSelectionIndex = 0;
    const int INVALID_PROG_INDEX = -1;

    bool setupSelection = false;

    void Awake() {
        // Load Deck
        cardNumberInDeck = new List<int>(SavedDeckManager.LoadDeck(deckToUseName));

        // Get managers refs
        shuffledDeckManager = FindObjectOfType<ShuffledDeckManager>();
        discardPileManager = FindObjectOfType<DiscardPileManager>();
        gameManager = FindObjectOfType<GameManager>();

        view = GetComponent<PhotonView>();

        deck = new Dictionary<CardType, List<Card>>();

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

        for(int i = 0; i < cardNumberInDeck.Count; i++) {
            deck.Add((CardType)i, new List<Card>());
            for(int j = 0; j < cardNumberInDeck[i]; j++) {
                allCards.Add(Instantiate(cardPrefab, Vector3.one * 1000.0f, Quaternion.identity));
                Card newCard = allCards[allCards.Count - 1].GetComponent<Card>();
                newCard.SetCardType((CardType)i);
                newCard.SetId(j);
                newCard.SetCardEffect(cardEffects[i]);
                newCard.UpdateCard(cardMaterials[i]);

                deck[(CardType)i].Add(newCard);
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
        List<int> cardsCount = cardNumberInDeck;

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
        for(int i = 0; i < deck.Keys.Count; i++) {
            copyCards.Add((CardType)i, new List<Card>(deck[(CardType)i]));
        }
        List<Card> drawPileCards = new List<Card>();
        int cardsCountdown = totalCards;

        int index = 0;

        while(true) {
            drawPileCards.Insert(0, copyCards[(CardType)_cardsOrder[index]][0]);
            drawPileCards[0].GetTransform().localPosition =
                new Vector3(Random.Range(-100.0f, 100.0f), 1000.0f, Random.Range(-100.0f, 100.0f));
            drawPileCards[0].GetTransform().parent = shuffledDeckManager.transform;
            copyCards[(CardType)_cardsOrder[index]].RemoveAt(0);

            index++;
            cardsCountdown--;
            if(cardsCountdown == 0)
                break;
        }

        shuffledDeckManager.InitializeDrawPile(drawPileCards);
    }
    #endregion

    // Give the designated player a certain number of card.
    public void DrawCardToPlayer(Player player, int numberOfCard, float timeToWait) {
        if(numberOfCard == 1) {
            Vector2Int index = player.GetHand().AddCard(shuffledDeckManager.DrawCard());
            player.handManager.AddCard(player.GetHand().cards[(CardType)index.x][index.y]);
        }
        else {
            StartCoroutine(DrawMultipleCard(player, numberOfCard, timeToWait));
        }
    }

    // Give multiple card 
    IEnumerator DrawMultipleCard(Player player, int numberOfCard, float timeToWait) {
        for(int i = 0; i < numberOfCard; i++) {
            Vector2Int index = player.GetHand().AddCard(shuffledDeckManager.DrawCard());
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
        // Register the player currently playing
        player = _player;

        // Register the card played
        cardPlayed = _cardPlayed;

        // Register the effect of the card
        currentCardEffect = _cardPlayed.GetCardEffect();

        // Register the order the card will play
        cardSelectionSteps = new List<CardSelectionStep>(currentCardEffect.GetCardSelectionSteps());
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
        cardPlayed = deck[(CardType)_cardPlayed.x][(int)_cardPlayed.y];
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

        turnProgress = TurnProgress.CARD_SHOWCASE;
    }

    public void CardPlayedProgress() {
        if(cardSelectionIndex != INVALID_PROG_INDEX) {
            if(cardSelectionSteps == null || cardSelectionIndex <= cardSelectionSteps.Count) {
                turnProgress = TurnProgress.UPDATE_ALL_PLAYER;
                cardSelectionIndex = INVALID_PROG_INDEX;
            }
        }

        switch(turnProgress) {
            case TurnProgress.DRAW_FIRST_CARD:
                break;

            case TurnProgress.SELECT_CARD_TO_PLAY:
                if(player != null && cardPlayed != null) {
                    if(player.id == gameManager.GetOwnerId()) {
                        cardPlayed.GetTransform().parent = cardShowcasePosition.transform;
                        initialRotation = cardPlayed.GetTransform().localRotation;
                        initialPos = cardPlayed.GetTransform().localPosition;

                        if(IsEffectNotPlayable()) {
                            effectNullified = true;
                            turnProgress = TurnProgress.UPDATE_ALL_PLAYER;
                            break;
                        }

                        setupSelection = true;
                        cardSelectionIndex = 0;
                    }
                }
                break;
            case TurnProgress.CARDS_SELECTION:
                switch(cardSelectionSteps[cardSelectionIndex]) {

                    case CardSelectionStep.SELECT_TARGET:
                        if(targetSelected) {
                            doSelectHand = false;
                            doSelectField = false;
                            targetSelected = false;

                            setupSelection = true;
                            cardSelectionIndex++;
                        }
                        break;

                    case CardSelectionStep.SELECT_TARGET_HAND_CARD:
                        if(setupSelection) {
                            setupSelection = false;
                            doSelectHand = true;
                            target.SetCardNumberToTarget(cardPlayed.GetCardEffect().GetTargetNumber());
                        }
                        target.SetTargetHand(true);
                        if(targetHandCardSelected) {
                            target.SetTargetHand(false);
                            targetHandCardSelected = false;

                            setupSelection = true;
                            cardSelectionIndex++;
                        }
                        break;

                    case CardSelectionStep.SELECT_TARGET_FIELD_CARD:
                        if(setupSelection) {
                            setupSelection = false;
                            doSelectField = true;

                            target.SetTargetField(true);
                            gameManager.SetCameraToPlayer(target.id, false);
                            target.SetCardNumberToTarget(cardPlayed.GetCardEffect().GetTargetNumber());
                        }
                        
                        if(targetFieldCardSelected) {
                            target.SetTargetField(false);
                            targetFieldCardSelected = false;

                            setupSelection = true;
                            cardSelectionIndex++;
                        }
                        break;

                    case CardSelectionStep.SELECT_PLAYER_HAND_CARD:
                        if(setupSelection) {
                            setupSelection = false;
                            gameManager.SetCameraToPlayer(player.id);

                            target.SetCardNumberToTarget(cardPlayed.GetCardEffect().GetTargetNumber());
                        }

                        turnProgress = TurnProgress.UPDATE_ALL_PLAYER;
                        setupSelection = true;
                        cardSelectionIndex++;
                        break;

                    case CardSelectionStep.SELECT_PLAYER_FIELD_CARD:
                        if(setupSelection) {
                            setupSelection = false;
                            gameManager.SetCameraToPlayer(player.id);

                            player.SetCardNumberToTarget(cardPlayed.GetCardEffect().GetTargetNumber());
                        }
                        turnProgress = TurnProgress.UPDATE_ALL_PLAYER;
                        setupSelection = true;
                        cardSelectionIndex++;
                        break;
                }
                break;

            case TurnProgress.UPDATE_ALL_PLAYER:
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
                turnProgress = TurnProgress.CARD_SHOWCASE;
                break;

            case TurnProgress.CARD_SHOWCASE:
                player.canPlay = false;
                moveFrameCountCurrent++;
                cardPlayed.GetTransform().localPosition = Vector3.Lerp(initialPos, Vector3.zero, (float)moveFrameCountCurrent / moveFrameCountTotal);
                cardPlayed.GetTransform().localRotation = Quaternion.Lerp(initialRotation, Quaternion.identity, (float)moveFrameCountCurrent / moveFrameCountTotal);
                if(moveFrameCountCurrent >= moveFrameCountTotal) {
                    if(effectNullified) {
                        turnProgress = TurnProgress.CARD_TO_FINAL_LOCATION;
                        break;
                    }
                    turnProgress = TurnProgress.EFFECT;
                }
                break;

            case TurnProgress.EFFECT:
                cardPlayed.GetCardEffect().DoEffect(
                    player,
                    target,
                    handCards.ToArray(),
                    fieldCards.ToArray(),
                    targetHandCards.ToArray(),
                    targetFieldCards.ToArray(),
                    this
                    );
                turnProgress = TurnProgress.CARD_TO_FINAL_LOCATION;
                break;

            case TurnProgress.CARD_TO_FINAL_LOCATION:
                if(CardLocationGoal(cardPlayed.GetCardType()) == CardEndLocaion.DISCARD_PILE && player.GetHand().cards.Count > 0) {
                    player.GetHand().RemoveCard(cardPlayed);
                    discardPileManager.DiscardCard(cardPlayed);
                    player.canPlay = true;
                    turnProgress = TurnProgress.END_OF_TURN;
                }
                else {
                    player.GetHand().RemoveCard(cardPlayed);
                    player.GetField().AddCard(cardPlayed);
                    player.fieldManager.AddCard(cardPlayed);
                    player.canPlay = true;
                    gameManager.NextPlayer();
                    turnProgress = TurnProgress.END_OF_TURN;
                }
                break;

            case TurnProgress.END_OF_TURN:
                player = null;
                cardPlayed = null;
                effectNullified = false;
                turnProgress = TurnProgress.SELECT_CARD_TO_PLAY;
                moveFrameCountCurrent = 0;
                break;
        }
    }

    public Card GetCard(Vector2 _cardInfo) {
        return deck[(CardType)_cardInfo.x][(int)_cardInfo.y];
    }
    public Card GetCard(CardType cardType, int id) {
        return deck[cardType][id];
    }

    #region Transfert Cards
    void TransferCardsOwnership(Player _source, Player _receiver, bool _isHandSource, bool _isHandReceiver, Vector2[] _cards) {
        StartCoroutine(TransferCardsOwnerShipRoutine(_source, _receiver, _isHandSource, _isHandReceiver, _cards));
    }

    IEnumerator TransferCardsOwnerShipRoutine(Player _source, Player _receiver, bool _isHandSource, bool _isHandReceiver, Vector2[] _cards) {
        for(int i = 0; i < _cards.Length; i++) {
            Card targetedCard = deck[(CardType)_cards[i].x][((int)_cards[i].y)];
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
            Card targetedCard = deck[(CardType)_cards[i].x][((int)_cards[i].y)];
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
    public TurnProgress GetProgressCardPlayed() {
        return turnProgress;
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

    private bool IsEffectNotPlayable() {
        return (cardSelectionSteps.Contains(CardSelectionStep.SELECT_TARGET_HAND_CARD) ?
                            players[1].GetHand().IsEmpty() &&
                            players[2].GetHand().IsEmpty() &&
                            players[3].GetHand().IsEmpty() :
                            false) ||
                (cardSelectionSteps.Contains(CardSelectionStep.SELECT_TARGET_FIELD_CARD) ?
                            players[1].GetField().IsEmpty() &&
                            players[2].GetField().IsEmpty() &&
                            players[3].GetField().IsEmpty() :
                            false) ||
                (cardSelectionSteps.Contains(CardSelectionStep.SELECT_PLAYER_HAND_CARD) ?
                            players[0].GetHand().IsEmpty() :
                            false) ||
                (cardSelectionSteps.Contains(CardSelectionStep.SELECT_PLAYER_FIELD_CARD) ?
                            players[0].GetField().IsEmpty() :
                            false);
    }

    public void SetCardEffect(CardEffect _cardEffect) {
        currentCardEffect = _cardEffect;
    }

    public void SetTurnProgresses(List<CardSelectionStep> _cardSelectionSteps) {
        cardSelectionSteps = _cardSelectionSteps;
    }
}