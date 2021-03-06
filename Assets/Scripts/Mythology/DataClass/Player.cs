using Photon.Pun;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviourPunCallbacks {

    // id of the player in the current room.
    public int id;

    PhotonView view;

    CardManager cardManager;
    GameManager gameManager;

    public Hand hand = new Hand();
    public Field field = new Field();

    [SerializeField] public HandManager handDisplayer;
    [SerializeField] public FieldManager fieldDisplayer;

    // Is true if it is the player turn.
    public bool canPlay = false;

    // is true if a card has been played by this player.
    public bool cardIsPlayed = false;

    // The card that the player just played.
    public Card cardPlayed;

    bool isSetup = false;

    public bool doTargetHand = false;
    public bool doTargetField = false;

    public bool doTargetType = false;

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

        handDisplayer = slotDistributor.GetHandSlots(localId);
        handDisplayer.transform.parent = transform;
        handDisplayer.SetPlayer(this);

        fieldDisplayer = slotDistributor.GetfieldSlots(localId);
        fieldDisplayer.transform.parent = transform;
        fieldDisplayer.SetPlayer(this);

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
            handDisplayer.UpdateCardPlayed(_card);
        }
    }

    public bool TargetHandCard(Vector2 _cardInfo) {
        return cardManager.SelectTargetHandCard(_cardInfo);
    }

    public bool TargetFieldCard(Vector2 _cardInfo) {
        return cardManager.SelectTargetFieldCard(_cardInfo);
    }

    [PunRPC]
    public void RPC_PlayCard(int _cardType, int _id) {
        Card cardPlayed = hand.cards[(CardManager.CardType)_cardType][0];
        for(int i = 0; i < hand.cards[(CardManager.CardType)_cardType].Count; i++) {
            if(hand.cards[(CardManager.CardType)_cardType][i].id == _id) {
                handDisplayer.UpdateCardPlayed(hand.cards[(CardManager.CardType)_cardType][i]);
            }
        }
    }

    public void IsPlayerTurn(bool _canPlay) {
        if(view.IsMine) {
            canPlay = _canPlay;
        }
    }
}
