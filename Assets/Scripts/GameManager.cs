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
    public const int EXCPECTED_PLAYER_NUMBER = 4;

    [SerializeField] Player[] players = new Player[EXCPECTED_PLAYER_NUMBER];
    CardManager cardManager;
    WaitingPanelManager WaitingPanelManager;
    PhotonView view;

    const int NO_ID = -1;
    int ownerId = NO_ID;
    bool[] availableIds = new bool[EXCPECTED_PLAYER_NUMBER];
    bool allPlayerConnected = false;
    bool playersInstantiated = false;

    bool startPile = true;

    NetworkSpawner networkSpawner;

    private void Awake() {
        view = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient) {
            view.RPC("RPC_Initialize", RpcTarget.AllBuffered);
        }
    }

#region Initialization and slot attribution
    [PunRPC]
    void RPC_Initialize() {
        view = GetComponent<PhotonView>();
        players = new Player[EXCPECTED_PLAYER_NUMBER];
        networkSpawner = FindObjectOfType<NetworkSpawner>();

        for(int i = 0; i < availableIds.Length; i++) {
            availableIds[i] = true;
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
                int id = Random.Range(0, EXCPECTED_PLAYER_NUMBER);
                if(!availableIds[id]) {
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
        availableIds[id] = false;

        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == (byte)EXCPECTED_PLAYER_NUMBER) {
            view.RPC("RPC_SpawnPlayers", RpcTarget.All);
        }
    }
#endregion

    private void Update() {
        if(allPlayerConnected) {

        }

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
                cardManager.DrawCardToPlayer(players[(playerTurn - ownerId) % EXCPECTED_PLAYER_NUMBER]);
                turnStep = TurnStep.PLAYER_TURN;
                players[(playerTurn - ownerId) % EXCPECTED_PLAYER_NUMBER].canPlay = true;
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

    public int GetOwnerId() {
        return ownerId;
    }

    public void Register(Player newPlayer, int playerId) {
        int localId = (playerId - ownerId) < 0 ? playerId - ownerId + EXCPECTED_PLAYER_NUMBER : playerId - ownerId;
        players[localId] = newPlayer;
    }

    public void ShowGame() {
        WaitingPanelManager.HideWaitingPanel();
    }

    public int GetExpectedPlayerNumber() {
        return EXCPECTED_PLAYER_NUMBER;
    }

    [PunRPC]
    void RPC_SpawnPlayers() {
        networkSpawner.SpawnPlayer();
    }
}
