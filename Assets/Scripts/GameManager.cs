using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
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

    [SerializeField] Player[] players;
    CardManager cardManager;
    WaitingPanelManager WaitingPanelManager;

    int ownerId;

    bool startPile = true;

    void Start() {
        cardManager = GetComponent<CardManager>();
        players = new Player[playerNumber];
    }

    private void Update() {
        if (startPile) {
            cardManager.InstantiateCards();
            startPile = false;
        }
        if(cardManager.initialHandGiven)
            TurnProgress();
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
