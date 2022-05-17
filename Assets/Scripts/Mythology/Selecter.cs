using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    CardManager cardManager;

    [SerializeField] GameObject leftPlayerButton;
    [SerializeField] GameObject middlePlayerButton;
    [SerializeField] GameObject rightPlayerButton;

    bool initialized = false;
    private void FixedUpdate() {
        if(!initialized)
            return;

        if(cardManager.GetProgressCardPlayed() == CardManager.ProgressCardPlayed.SELECT_TARGET) {
            leftPlayerButton.SetActive(true);
            middlePlayerButton.SetActive(true);
            rightPlayerButton.SetActive(true);
        }
        else {
            leftPlayerButton.SetActive(false);
            middlePlayerButton.SetActive(false);
            rightPlayerButton.SetActive(false);

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
