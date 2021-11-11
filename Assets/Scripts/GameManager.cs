using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum TurnStep
    {
        PAUSE,
        START_OF_TURN,
        ONE_MORE_CARD,
        WAITING_FOR_CARD,
        CARD_PLAY,
        EFFECT,
        END_OF_TURN
    }

    public TurnStep turnStep = TurnStep.PAUSE;

    [SerializeField] List<Player> players;
    [SerializeField] List<int> playerTurns = new List<int>();
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
                players[playerTurns[0]].canPlay = true;
                cardManager.DrawCardToPlayer(players[playerTurns[0]]);
                turnStep = TurnStep.WAITING_FOR_CARD;
                break;

            case TurnStep.ONE_MORE_CARD:
                turnStep = TurnStep.WAITING_FOR_CARD;
                players[playerTurns[0]].cardPlayed = null;
                break;

            case TurnStep.WAITING_FOR_CARD:
                if (players[playerTurns[0]].cardIsPlayed) {
                    players[playerTurns[0]].canPlay = false;
                    turnStep = TurnStep.CARD_PLAY;
                }
                break;

            case TurnStep.CARD_PLAY:
                if (cardManager.WhereCardGoing(players[playerTurns[0]].cardPlayed.cardType) == CardManager.CardEndLocaion.DISCARD_PILE) {
                    turnStep = TurnStep.ONE_MORE_CARD;
                }
                break;

            case TurnStep.EFFECT:
                turnStep = TurnStep.END_OF_TURN;
                break;

            case TurnStep.END_OF_TURN:
                players[playerTurns[0]].cardPlayed = null;
                playerTurns.Add(playerTurns[0]);
                playerTurns.RemoveAt(0);
                turnStep = TurnStep.START_OF_TURN;
                break;
        }
    }

}
