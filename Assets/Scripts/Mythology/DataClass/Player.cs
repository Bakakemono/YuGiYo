using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviourPunCallbacks {

    // id of the player in the current room.
    public int id;

    PhotonView view;

    private CardManager cardManager;
    private GameManager gameManager;

    private Hand hand = new Hand();
    private Field field = new Field();

    [SerializeField] public HandManager handManager;
    [SerializeField] public FieldManager fieldManager;

    // Is true if it is the player turn.
    public bool canPlay = false;

    // is true if a card has been played by this player.
    public bool cardIsPlayed = false;

    // The card that the player just played.
    public Card cardPlayed;

    bool isSetup = false;

    private bool doTargetHand = false;
    private bool doTargetField = false;

    private bool doTargetType = false;

    private int nmbCardToTarget = 0;
    
    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        cardManager = FindObjectOfType<CardManager>();
        view = GetComponent<PhotonView>();

        if(view.IsMine) {
            id = gameManager.GetOwnerId();
            view.RPC("RPC_Setup", RpcTarget.All, id);
        }
    }

    [PunRPC]
    void RPC_Setup(int _idUpdated) {
        id = _idUpdated;
        gameManager = FindObjectOfType<GameManager>();
        int ownerId = gameManager.GetOwnerId();
        gameManager.Register(this, id);
        SlotDistributor slotDistributor = FindObjectOfType<SlotDistributor>();
        int localId = (id - ownerId) < 0 ? id - ownerId + GameManager.EXPECTED_PLAYER_NUMBER : id - ownerId;
        Transform slotTransform = slotDistributor.GetPlayerSlots(localId).transform;
        transform.position = slotTransform.position;
        transform.rotation = slotTransform.rotation;

        handManager = slotDistributor.GetHandSlots(localId);
        handManager.transform.parent = transform;
        handManager.SetPlayer(this);

        fieldManager = slotDistributor.GetfieldSlots(localId);
        fieldManager.transform.parent = transform;
        fieldManager.SetPlayer(this);

        isSetup = true;
    }

    public void SetPlayerTurn(bool _isPlayerTurn) {
        canPlay = _isPlayerTurn;
    }

    public void PlayCard(Card _card) {
        if(view.IsMine) {
            cardManager.PlayCard(this, _card);
        }
        else {
            handManager.UpdateCardPlayed(_card);
        }
    }

    public bool TargetHandCard(Vector2 _cardInfo) {
        return cardManager.SelectTargetHandCard(_cardInfo);
    }

    public bool TargetFieldCard(List<Vector2> _cardsInfos) {
        return cardManager.SelectTargetFieldCard(_cardsInfos);
    }

    [PunRPC]
    public void RPC_PlayCard(int _cardType, int _id) {
        Card cardPlayed = hand.cards[(CardManager.CardType)_cardType][0];
        for(int i = 0; i < hand.cards[(CardManager.CardType)_cardType].Count; i++) {
            if(hand.cards[(CardManager.CardType)_cardType][i].GetId() == _id) {
                handManager.UpdateCardPlayed(hand.cards[(CardManager.CardType)_cardType][i]);
            }
        }
    }

    public void IsPlayerTurn(bool _canPlay) {
        if(view.IsMine) {
            canPlay = _canPlay;
        }
    }

    public bool IsHandTargeted() {
        return doTargetHand;
    }

    public void SetTargetHand(bool _doTargetHand) {
        doTargetHand = _doTargetHand;
    }

    public bool IsFieldTargeted() {
        return doTargetField;
    }

    public void SetTargetField(bool _doTargetField) {
        doTargetField = _doTargetField;
    }

    public bool IsTypeTargeted() {
        return doTargetType;
    }

    public void SetTargetType(bool _doTargetType) {
        doTargetType = _doTargetType;
    }

    public Hand GetHand() {
        return hand;
    }

    public Field GetField() {
        return field;
    }

    public void AddCardtoHand(Card _card) {
        hand.AddCard(_card);
        handManager.AddCard(_card);
    }

    public void RemoveCardFromHand(Card _card) {
        handManager.RemoveCard(_card);
        hand.RemoveCard(_card);
    }

    public void AddCardtoField(Card _card) {
        field.AddCard(_card);
        fieldManager.AddCard(_card);
    }

    public void RemoveCardFromField(Card _card) {
        fieldManager.RemoveCard(_card);
        field.RemoveCard(_card);
    }

    List<Vector2> GetRandomCardsFromHand(int _nmbCards) {
        List<Vector2> cards = new List<Vector2>();
        return cards;
    }

    // Reset player card including Hand Manager and Field Manager.
    public void ResetPlayer() {
        handManager.ResetHandManager();
        hand.ResetHand();

        fieldManager.ResetFieldManager();
        field.ResetField();
    }
}
