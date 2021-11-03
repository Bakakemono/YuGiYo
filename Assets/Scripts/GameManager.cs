using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    List<Material> cardMaterials = new List<Material>();

    [SerializeField] GameObject card;

    DrawPileManager drawPileManager;
    CardManager cardManager;

    bool startPile = true;

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
        cardManager = GetComponent<CardManager>();
    }

    private void Update() {
        if (startPile) {
            cardManager.InstantiateCards();
            startPile = false;
        }
    }
}
