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

    public TurnStep turnStep = TurnStep.PAUSE;

    public int playerTurn = 0;

    [SerializeField] List<Player> players;
    CardManager cardManager;

    bool startPile = true;
    bool cardPlayed = false;

    void Start() {
        cardManager = GetComponent<CardManager>();
    }

    private void Update() {
        if (startPile) {
            cardManager.InstantiateCards();
            startPile = false;
        }
    }

    void TurnProgress() {
        switch (turnStep) {
            case TurnStep.PAUSE:
                break;
            case TurnStep.START_OF_TURN:
                players[playerTurn].canPlay = true;
                cardManager.DrawCardToPlayer(players[playerTurn]);
                turnStep = TurnStep.PLAYER_TURN;
                break;

            case TurnStep.PLAYER_TURN:
                break;
            case TurnStep.NEXT_TURN:
                players[playerTurn].canPlay = false;
                playerTurn++;
                playerTurn = playerTurn % players.Count;
                turnStep = TurnStep.START_OF_TURN;
                break;
        }
    }

    public void NextPlayer() {
        turnStep = TurnStep.NEXT_TURN;
    }

}
