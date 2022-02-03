using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        WAITING_FOR_CARD,
        CARD_IS_SHOWN,
        CARD_PLAYED,
        RESET
    }

    ProgressCardPlayed progressCardPlayed = ProgressCardPlayed.WAITING_FOR_CARD;

    [SerializeField] GameObject card;

    Dictionary<CardType, List<Card>> cards;

    List<int> cardsCount;

    [SerializeField] List<Material> cardMaterials = new List<Material>();

    // Components
    PhotonView view;

    // External Manager
    GameManager gameManager;
    DrawPileManager drawPileManager;
    DiscardPileManager discardPileManager;

    [SerializeField] Player[] players;

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
    }

    private void FixedUpdate() {
        if(drawPileManager.drawPileInstantiated && !startDistributingFirstHand) {
            startDistributingFirstHand = true;
            StartCoroutine(DrawInitialHand());
        }
        if (initialHandGiven) {
            CardPlayedProgress();
        }
    }

    public void InstantiateCards() {
        Debug.Log("InstantiateCards");
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

        view.RPC("RPC_PrepareCardsOrder", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_PrepareCardsOrder() {
        Debug.Log("RPC_PrepareCardsOrder");

        List<CardType> allTypes = new List<CardType>();
        List<CardType> cardsOrder = new List<CardType>();
        cardsCount = cardsNumber; 

        for(CardType type = 0; type != CardType.Length; type++) {
            allTypes.Add(type);
        }

        while(true) {
            int typeSelected = Random.Range(0, allTypes.Count);

            cardsOrder.Add(allTypes[typeSelected]);

            cardsCount[typeSelected]--;
            if(cardsCount[typeSelected] == 0) {
                cardsCount.RemoveAt(typeSelected);
                allTypes.RemoveAt(typeSelected);
                if(allTypes.Count == 0) {
                    break;
                }
            }
        }

        view.RPC("RPC_InitializeDrawPile", RpcTarget.All, cardsOrder);
    }

    [PunRPC]
    void RPC_InitializeDrawPile(List<CardType> _cardsOrder) {
        Debug.Log("RPC_InitializeDrawPile");

        Dictionary<CardType, List<Card>> copyCards = cards;
        List<Card> drawPileCards = new List<Card>();
        int totalCards = _cardsOrder.Count;

        while(true) {
            drawPileCards.Insert(0, copyCards[_cardsOrder[0]][0]);
            drawPileCards[0].customTransform.localPosition = 
                new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(0.0f, 30.0f), Random.Range(-100.0f, 100.0f));
            drawPileCards[0].customTransform.parent = drawPileManager.transform;
            copyCards[_cardsOrder[0]].RemoveAt(0);
            _cardsOrder.RemoveAt(0);

            totalCards--;
            if(totalCards == 0)
                break;
        }

        drawPileManager.InitializeDrawPile(drawPileCards);
    }

    public void DrawCardToPlayer(Player player) {
        Vector2Int index = player.hand.AddCard(drawPileManager.DrawCard());
        player.handDisplayer.AddCardToDisplay(player.hand.cards[(CardType)index.x][index.y]);
    }

    IEnumerator DrawInitialHand() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < startingCardNumber; i++) {
            for(int j = 0; j < players.Length; j++) {
                DrawCardToPlayer(players[(gameManager.playerTurn + j) % GameManager.EXPECTED_PLAYER_NUMBER]);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return new WaitForSeconds(4.0f);
        initialHandGiven = true;
    }

    public CardEndLocaion CardLocaionGoal(CardType cardType) {
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

    public void CardPlayedProgress() {
        switch (progressCardPlayed) {
            case ProgressCardPlayed.WAITING_FOR_CARD:
                if(player != null && cardPlayed != null) {
                    frameCountCurrent = 0;
                    cardPlayed.customTransform.parent = cardShowcasePosition.transform;
                    initialRotation = cardPlayed.customTransform.localRotation;
                    initialPos = cardPlayed.customTransform.localPosition;
                    progressCardPlayed = ProgressCardPlayed.CARD_IS_SHOWN;
                }
                break;

            case ProgressCardPlayed.CARD_IS_SHOWN:
                player.canPlay = false;
                frameCountCurrent++;
                cardPlayed.customTransform.localPosition = Vector3.Lerp(initialPos, Vector3.zero, (float)frameCountCurrent / frameMovementTotal);
                cardPlayed.customTransform.localRotation = Quaternion.Lerp(initialRotation, Quaternion.identity, (float)frameCountCurrent / frameMovementTotal);
                if (frameCountCurrent == frameGobaleTotal) {
                    progressCardPlayed = ProgressCardPlayed.CARD_PLAYED;
                }
                break;

            case ProgressCardPlayed.CARD_PLAYED:
                if(CardLocaionGoal(cardPlayed.cardType) == CardEndLocaion.DISCARD_PILE && player.hand.cards.Count > 0) {
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
                progressCardPlayed = ProgressCardPlayed.RESET;
                break;

            case ProgressCardPlayed.RESET:
                player = null;
                cardPlayed = null;
                progressCardPlayed = ProgressCardPlayed.WAITING_FOR_CARD;
                break;
        }
    }

    public void RegisterPlayers(Player[] gmPlayers) {
        players = gmPlayers;
    }

}