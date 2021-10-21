using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    List<int> cardsNumber = new List<int> {
        16,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        8,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        12
    };

    [SerializeField]
    List<Material> cardMaterials = new List<Material>();

    [SerializeField] GameObject card;

    DrawPileManager drawPileManager;

    bool startPile = true;

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
    }

    private void Update() {
        if (startPile) {
            InstantiateCards();
            startPile = false;
        }
    }

    void InstantiateCards() {
        List<GameObject> allCards = new List<GameObject>();

        for (int i = 0; i < cardsNumber.Count; i++) {
            for (int j = 0; j < cardsNumber[i]; j++) {
                allCards.Add(Instantiate(card, Vector3.one * 1000.0f, Quaternion.identity));
                allCards[allCards.Count - 1].GetComponent<Card>().cardType = (CardManager.CardType)i;
                allCards[allCards.Count - 1].GetComponent<Card>().UpdateCard(cardMaterials[i]);
            }
        }

        drawPileManager.InitializeDrawPile(allCards);
    }
}