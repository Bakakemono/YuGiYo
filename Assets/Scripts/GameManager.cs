using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] List<Player> players;
    [SerializeField] List<int> playerTurns = new List<int>();
    CardManager cardManager;

    bool startPile = true;

    void Start() {
        cardManager = GetComponent<CardManager>();
    }

    private void Update() {
        if (startPile) {
            cardManager.InstantiateCards();
            startPile = false;
        }
    }


}
