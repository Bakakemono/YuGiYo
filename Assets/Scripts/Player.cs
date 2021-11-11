using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour {
    public Hand hand = new Hand();
    public Field field = new Field();

    [SerializeField] public HandDisplayer handDisplayer;
    [SerializeField] public FieldDisplayer fieldDisplayer;

    CardManager cardManager;

    public bool canPlay = false;

    public bool cardIsPlayed = false;
    public Card cardPlayed;

    private void Start() {
        cardManager = FindObjectOfType<CardManager>();
        handDisplayer.SetPlayer(this);
    }

    public void SetPlayerTurn(bool isPlayerTurn) {
        canPlay = isPlayerTurn;
    }

    public void PlayCard(Card card) {

    }
}
