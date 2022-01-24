using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks {
    public enum TurnStep
    {
        PAUSE,
        START_OF_TURN,
        PLAYER_TURN,
        NEXT_TURN
    }

    public TurnStep turnStep = TurnStep.START_OF_TURN;

    public int playerTurn = 0;
    const int playerNumber = 4;

    [SerializeField] Player[] players = new Player[playerNumber];
    CardManager cardManager;
    WaitingPanelManager WaitingPanelManager;
    PhotonView view;

    const int NO_ID = -1;
    int ownerId = NO_ID;
    bool[] availableIds = new bool[playerNumber];

    bool startPile = true;

    NetworkSpawner networkSpawner;

    private void Awake() {
        if(PhotonNetwork.IsMasterClient) {
            view.RPC("RPC_Initialize", RpcTarget.AllBuffered);
        }
    }

#region Initialization and slot attribution
    [PunRPC]
    void RPC_Initialize() {
        players = new Player[playerNumber];
        view = GetComponent<PhotonView>();
        networkSpawner = FindObjectOfType<NetworkSpawner>();

        for(int i = 0; i < availableIds.Length; i++) {
            availableIds[i] = false;
        }

        cardManager = FindObjectOfType<CardManager>();
        WaitingPanelManager = FindObjectOfType<WaitingPanelManager>();
        view.RPC("RPC_UpdateAvailableSlot", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_UpdateAvailableSlot() {
        view.RPC("RPC_SendAndSelectAvailableSlot", RpcTarget.All, availableIds);
    }

    [PunRPC]
    void RPC_SendAndSelectAvailableSlot(bool[] availableIdsUpdated) {
        availableIds = availableIdsUpdated;
        if(ownerId == NO_ID) {
            while(true) {
                int id = Random.Range(0, playerNumber);
                if(availableIds[id]) {
                    continue;
                }
                ownerId = id;
                WaitingPanelManager.id.text = ownerId.ToString();
                view.RPC("RPC_RegisterSelectedSlot", RpcTarget.MasterClient, id);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_RegisterSelectedSlot(int id) {
        availableIds[id] = true;
    }
#endregion

    private void Update() {
        //if (startPile) {
        //    cardManager.InstantiateCards();
        //    startPile = false;
        //}
        //if(cardManager.initialHandGiven)
        //    TurnProgress();
    }

    void TurnProgress() {
        switch (turnStep) {
            case TurnStep.PAUSE:
                break;

            case TurnStep.START_OF_TURN:
                cardManager.DrawCardToPlayer(players[(playerTurn - ownerId) % playerNumber]);
                turnStep = TurnStep.PLAYER_TURN;
                players[(playerTurn - ownerId) % playerNumber].canPlay = true;
                break;

            case TurnStep.PLAYER_TURN:
                break;

            case TurnStep.NEXT_TURN:
                players[playerTurn].canPlay = false;
                playerTurn++;
                playerTurn = playerTurn % players.Length;
                turnStep = TurnStep.START_OF_TURN;
                break;
        }
    }

    public void NextPlayer() {
        turnStep = TurnStep.NEXT_TURN;
    }


    public Player GetPlayer(int id) {
        return players[id];
    }

    public void OwnerRegister(int id) {
        ownerId = id;
    }

    public void Register(Player newPlayer, int playerId) {
        players[(playerId - ownerId) % playerNumber] = newPlayer;
    }

    public void ShowGame() {
        WaitingPanelManager.HideWaitingPanel();
    }
}
