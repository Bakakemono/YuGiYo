using Photon.Pun;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviourPunCallbacks {
    public Hand hand = new Hand();
    public Field field = new Field();

    [SerializeField] public HandDisplayer handDisplayer;
    [SerializeField] public FieldDisplayer fieldDisplayer;

    CardManager cardManager;
    GameManager gameManager;

    public bool canPlay = false;

    public bool cardIsPlayed = false;
    public Card cardPlayed;

    public int id;
    PhotonView view;

    bool isSetup = false;

    [SerializeField] Color playersColor;

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        cardManager = FindObjectOfType<CardManager>();
        view = GetComponent<PhotonView>();

        if(view.IsMine) {
            id = gameManager.GetOwnerId();
            view.RPC("RPC_Setup", RpcTarget.All, id);
        }
    }

    private void Update() {
        
    }

    public void SetPlayerTurn(bool isPlayerTurn) {
        canPlay = isPlayerTurn;
    }

    public void PlayCard(Card card) {
        cardManager.PlayCard(this, card);
    }

    [PunRPC]
    void RPC_Setup(int idUpdated) {
        id = idUpdated;
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



        isSetup = true;
    }

    private void OnDrawGizmos() {
        if(!isSetup)
            return;

        switch(id) {
            case 0:
                Gizmos.color = Color.blue;
                break;
            case 1:
                Gizmos.color = Color.red;
                break;
            case 2:
                Gizmos.color = Color.yellow;
                break;
            case 3:
                Gizmos.color = new Color(51.0f/ 255.0f, 214.0f / 255.0f, 151.0f / 255.0f, 1);
                break;
        }

        playersColor = Gizmos.color;
        Gizmos.DrawCube(fieldDisplayer.transform.position, Vector3.one);
    }
}
