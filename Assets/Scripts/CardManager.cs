using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CardPlayedData {
    
    Vector2 cardPlayed;

    // Raw index
    int targetSelected;
    Vector2 targetHandCard;
    Vector2 targetFieldCard;
    Vector2 handCard;
    Vector2 fieldCard;
}

public class CardManager : MonoBehaviourPunCallbacks
{
    public enum CardType {
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

    public enum CardEndLocaion
    {
        FIELD,
        DISCARD_PILE
    }

    public enum ProgressCardPlayed
    {
        DRAW_FIRST_CARD,
        SELECT_CARD_TO_PLAY, 
        SELECT_TARGET,
        SELECT_TARGET_HAND_CARD,
        SELECT_TARGET_FIELD_CARD,
        SELECT_HAND_CARD,
        SELECT_FIELD_CARD,
        UPDATE_ALL_PLAYER,
        ANIMATION_PLAYIING,
        END_OF_TURN
    }

    ProgressCardPlayed progressCardPlayed = ProgressCardPlayed.SELECT_CARD_TO_PLAY;

    [SerializeField] GameObject card;

    Dictionary<CardType, List<Card>> cards;

    [SerializeField] List<Material> cardMaterials = new List<Material>();

    // Components
    PhotonView view;

    // External Manager
    GameManager gameManager;
    DrawPileManager drawPileManager;
    DiscardPileManager discardPileManager;

    // List of all players
    [SerializeField] Player[] players;

    // Number of card that each player will get at the start of the 
    [SerializeField] int startingCardNumber = 6;

    bool startDistributingFirstHand = false;

    public bool initialHandGiven = false;

    [SerializeField] Transform cardShowcasePosition;

    Card cardPlayed = null;
    Player player = null;

    int frameMovementTotal = 50;
    int frameGobaleTotal = 75;
    int frameCountCurrent = 0;

    Vector3 initialPos;
    Quaternion initialRotation;

    int totalCards;


    void Awake() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
        discardPileManager = FindObjectOfType<DiscardPileManager>();
        gameManager = FindObjectOfType<GameManager>();
        view = GetComponent<PhotonView>();

        cards = new Dictionary<CardType, List<Card>>();

        cardShowcasePosition = FindObjectOfType<SlotDistributor>().GetShowCasePosition();
    }

    private void FixedUpdate() {

        if (initialHandGiven) {
            CardPlayedProgress();
        }
    }

    #region Draw Pile Creation
    // Instantiate all the card that will be played during the game.
    public void InstantiateCards() {
        List<GameObject> allCards = new List<GameObject>();

        for (int i = 0; i < cardsNumber.Count; i++) {
            cards.Add((CardType)i, new List<Card>());
            for (int j = 0; j < cardsNumber[i]; j++) {
                allCards.Add(Instantiate(card, Vector3.one * 1000.0f, Quaternion.identity));
                Card newCard = allCards[allCards.Count - 1].GetComponent<Card>();
                newCard.cardType = (CardType)i;
                newCard.id = j;
                newCard.UpdateCard(cardMaterials[i]);

                cards[(CardType)i].Add(newCard);
            }
        }
        totalCards = allCards.Count;

        if(PhotonNetwork.IsMasterClient) {
            RPC_PrepareCardsOrder();
        }
    }

    // The master client shuffle the cards and then send the order to each players.
    [PunRPC]
    void RPC_PrepareCardsOrder() {
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
        Debug.Log("RPC_InitializeDrawPile");
        Dictionary<CardType, List<Card>> copyCards = cards;
        List<Card> drawPileCards = new List<Card>();
        int cardsCountdown = totalCards;

        int index = 0;

        while(true) {
            drawPileCards.Insert(0, copyCards[(CardType)_cardsOrder[index]][0]);
            drawPileCards[0].customTransform.localPosition = 
                new Vector3(Random.Range(-100.0f, 100.0f), 1000.0f, Random.Range(-100.0f, 100.0f));
            drawPileCards[0].customTransform.parent = drawPileManager.transform;
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
        Vector2Int index = player.hand.AddCard(drawPileManager.DrawCard());
        player.handDisplayer.AddCardToHand(player.hand.cards[(CardType)index.x][index.y]);
        }
        else {
            StartCoroutine(DrawMultipleCard(player, numberOfCard, timeToWait));
        }
    }

    IEnumerator DrawMultipleCard(Player player, int numberOfCard, float timeToWait) {
        for(int i = 0; i < numberOfCard; i++) {
            Vector2Int index = player.hand.AddCard(drawPileManager.DrawCard());
            player.handDisplayer.AddCardToHand(player.hand.cards[(CardType)index.x][index.y]);
            yield return new WaitForSeconds(timeToWait);
        }
    }

    // Give the cards each player should have at the begining of the game.
    IEnumerator DrawInitialHand() {
        for (int i = 0; i < startingCardNumber; i++) {
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
        switch (cardType) {
            case CardType.BEER:
                return CardEndLocaion.DISCARD_PILE;

            default:
                return CardEndLocaion.FIELD;
        }
    }


    public void PlayCard(Player pl, Card cd) {
        player = pl;
        cardPlayed = cd;
    }

    [PunRPC]
    void RPC_UpdatePlayers(CardPlayedData cardPlayedData) {
        player = players[gameManager.playerTurn];
        //cardPlayed = 
    }

    public void CardPlayedProgress() {
        switch(progressCardPlayed) {
            case ProgressCardPlayed.DRAW_FIRST_CARD:
                break;

            case ProgressCardPlayed.SELECT_CARD_TO_PLAY:
                if(player != null && cardPlayed != null) {
                    if(view.IsMine) {
                        frameCountCurrent = 0;
                        cardPlayed.customTransform.parent = cardShowcasePosition.transform;
                        initialRotation = cardPlayed.customTransform.localRotation;
                        initialPos = cardPlayed.customTransform.localPosition;

                        if(cardPlayed.cardEffect.DoTargetPlayer()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_TARGET;
                            break;
                        }
                        else if(cardPlayed.cardEffect.DoSelectCard()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                            break;
                        }
                        else if(cardPlayed.cardEffect.DoSelectField()) {
                            progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                            break;
                        }

                        progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                    }
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET:
                if(cardPlayed.cardEffect.DoTargetHand()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_TARGET_HAND_CARD;
                    break;
                }
                else if(cardPlayed.cardEffect.DoTargetField()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_TARGET_FIELD_CARD;
                    break;
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET_HAND_CARD:
                if(cardPlayed.cardEffect.DoSelectCard()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                    break;
                }
                else if (cardPlayed.cardEffect.DoSelectField()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                    break;
                }
                break;

            case ProgressCardPlayed.SELECT_TARGET_FIELD_CARD:
                if(cardPlayed.cardEffect.DoSelectCard()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_HAND_CARD;
                    break;
                }
                else if(cardPlayed.cardEffect.DoSelectField()) {
                    progressCardPlayed = ProgressCardPlayed.SELECT_FIELD_CARD;
                    break;
                }
                break;

            case ProgressCardPlayed.SELECT_HAND_CARD:
                progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                break;

            case ProgressCardPlayed.SELECT_FIELD_CARD:
                progressCardPlayed = ProgressCardPlayed.UPDATE_ALL_PLAYER;
                break;

            case ProgressCardPlayed.UPDATE_ALL_PLAYER:
                progressCardPlayed = ProgressCardPlayed.ANIMATION_PLAYIING;
                break;

            case ProgressCardPlayed.ANIMATION_PLAYIING:
                if(frameCountCurrent != frameGobaleTotal) {
                    player.canPlay = false;
                    frameCountCurrent++;
                    cardPlayed.customTransform.localPosition = Vector3.Lerp(initialPos, Vector3.zero, (float)frameCountCurrent / frameMovementTotal);
                    cardPlayed.customTransform.localRotation = Quaternion.Lerp(initialRotation, Quaternion.identity, (float)frameCountCurrent / frameMovementTotal);
                    break;
                }
                if(CardLocationGoal(cardPlayed.cardType) == CardEndLocaion.DISCARD_PILE && player.hand.cards.Count > 0) {
                    player.hand.RemoveCard(cardPlayed);
                    discardPileManager.DiscardCard(cardPlayed);
                    player.canPlay = true;
                }
                else {
                    player.hand.RemoveCard(cardPlayed);
                    player.field.AddCard(cardPlayed);
                    player.fieldDisplayer.AddCardToField(cardPlayed);
                    player.canPlay = true;
                    gameManager.NextPlayer();
                }
                break;

            case ProgressCardPlayed.END_OF_TURN:
                player = null;
                cardPlayed = null;
                progressCardPlayed = ProgressCardPlayed.SELECT_CARD_TO_PLAY;
                break;
        }
    }

    public void RegisterPlayers(Player[] gmPlayers) {
        players = gmPlayers;
    }

    public void StartGivingInitialHand() {
        StartCoroutine(DrawInitialHand());
    }
}