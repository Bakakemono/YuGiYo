using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    CardManager cardManager;

    [SerializeField] GameObject[] playerSelectionButtons;
    bool updated = false;

    bool initialized = false;
    private void FixedUpdate() {
        if(!initialized)
            return;

        if(!updated && cardManager.GetDoSelectHand()) {
            for(int i = 1; i < cardManager.players.Length; i++) {
                if(!cardManager.players[i].hand.IsEmpty()) {
                    playerSelectionButtons[i - 1].SetActive(true);
                }
            }
            updated = true;
        }
        else if(!updated && cardManager.GetDoSelectField()) {
            for(int i = 1; i < cardManager.players.Length; i++) {
                if(!cardManager.players[i].field.IsEmpty()) {
                    playerSelectionButtons[i - 1].SetActive(true);
                }
            }
            updated = true;
        }
        else if(updated && !cardManager.GetDoSelectHand() && !cardManager.GetDoSelectField()) {
            for(int i = 1; i < cardManager.players.Length; i++) {
                playerSelectionButtons[i - 1].SetActive(false);
            }
            updated = false;
        }
    }

    public void SelectLeftPlayer() {
        cardManager.SelectTarget(1);
    }

    public void SelectMiddlePlayer() {
        cardManager.SelectTarget(2);
    }

    public void SelectRightPlayer() {
        cardManager.SelectTarget(3);
    }

    public void Initialize() {
        cardManager = FindObjectOfType<CardManager>();
        initialized = true;
    }
}
