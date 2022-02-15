using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks {
    public enum TurnStep {
        PAUSE,
        START_OF_TURN,
        PLAYER_TURN,
        NEXT_TURN
    }

    public TurnStep turnStep = TurnStep.START_OF_TURN;

    public int playerTurn = 0;
    public const int EXPECTED_PLAYER_NUMBER = 4;

    [SerializeField] Player[] players = new Player[EXPECTED_PLAYER_NUMBER];
    CardManager cardManager;
    WaitingPanelManager waitingPanelManager;
    PhotonView view;

    const int NO_ID = -1;
    int ownerId = NO_ID;
    bool[] availableIds = new bool[EXPECTED_PLAYER_NUMBER];
    bool allPlayerConnected = false;
    bool playersInstantiated = false;

    bool startPile = true;

    NetworkSpawner networkSpawner;

    int timerTotalTime = 3;
    int timeEndTime;
    bool timerStart = false;
    bool timerEnd = false;

    bool gameStarted = false;

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
        players = new Player[EXPECTED_PLAYER_NUMBER];
        networkSpawner = FindObjectOfType<NetworkSpawner>();

        for(int i = 0; i < availableIds.Length; i++) {
            availableIds[i] = true;
        }

        waitingPanelManager = FindObjectOfType<WaitingPanelManager>();

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
                int id = Random.Range(0, EXPECTED_PLAYER_NUMBER);
                if(!availableIds[id]) {
                    continue;
                }
                ownerId = id;
                waitingPanelManager.id.text = ownerId.ToString();
                view.RPC("RPC_RegisterSelectedSlot", RpcTarget.MasterClient, id);
                break;
            }
        }
    }

    [PunRPC]
    void RPC_RegisterSelectedSlot(int id) {
        availableIds[id] = false;

        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == (byte)EXPECTED_PLAYER_NUMBER) {
            networkSpawner.SpawnCardManager();
            view.RPC("RPC_SpawnPlayers", RpcTarget.All);
            view.RPC("RPC_StartTimer",
                RpcTarget.All,
                (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc)).TotalSeconds + timerTotalTime);
        }
    }

    [PunRPC]
    void RPC_StartTimer(int endTime) {
        timeEndTime = endTime;  
        timerStart = true;
    }
#endregion

    private void Update() {
        if(timerStart && !timerEnd) {
            int time = (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
            waitingPanelManager.SetTimer(timeEndTime - time);
            if(timeEndTime < time) {
                timerEnd = true;
                waitingPanelManager.HideWaitingPanel();
            }
            return;
        }

        if(gameStarted) {
            if(startPile) {
                startPile = false;
                cardManager.RegisterPlayers(players);
                playerTurn = (0 - ownerId) < 0 ? 0 - ownerId + EXPECTED_PLAYER_NUMBER : 0 - ownerId;
                cardManager.InstantiateCards();


            }
            if(cardManager.initialHandGiven)
                TurnProgress();
        }
    }

    void TurnProgress() {
        switch (turnStep) {
            case TurnStep.PAUSE:
                break;

            case TurnStep.START_OF_TURN:
                cardManager.DrawCardToPlayer(players[playerTurn], 1, 0.0f);
                turnStep = TurnStep.PLAYER_TURN;
                players[playerTurn].IsPlayerTurn(true);
                break;

            case TurnStep.PLAYER_TURN:
                break;

            case TurnStep.NEXT_TURN:
                players[playerTurn].IsPlayerTurn(false);
                playerTurn++;
                playerTurn = playerTurn % players.Length;
                turnStep = TurnStep.START_OF_TURN;

                // Debug indicator
                if(playerTurn == 0) {
                    waitingPanelManager.indicator.color = Color.green;
                }
                else {
                    waitingPanelManager.indicator.color = Color.red;
                }

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
        int localId = (playerId - ownerId) < 0 ? playerId - ownerId + EXPECTED_PLAYER_NUMBER : playerId - ownerId;
        players[localId] = newPlayer;
        if(!PhotonNetwork.IsMasterClient)
            return;

        int playerNumber = 0;
        foreach(Player player in players) {
            if(player != null)
                playerNumber++;
        }
        if(playerNumber == EXPECTED_PLAYER_NUMBER)
            view.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SpawnPlayers() {
        networkSpawner.SpawnPlayer();
    }

    [PunRPC]
    void StartGame() {
        cardManager = FindObjectOfType<CardManager>();
        gameStarted = true;
    }
}
