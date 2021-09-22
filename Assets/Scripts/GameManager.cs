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

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
    }

    void InstantiateMaterials() {
        for (int i = 0; i < cardMaterials.Count; i++) {
            cardMaterials[i].SetTextureOffset(
                0,
                new Vector2((float)i % Card.cardColumnNmb * Card.cardColumnDecal,
                Mathf.FloorToInt((float)i / Card.cardRawNmb) * Card.cardRawDecal));
        }
    }

    void InstantiateCards() {
        List<GameObject[]> allCards = new List<GameObject[]>();

        for (int i = 0; i < cardsNumber.Count; i++) {
            allCards.Add(new GameObject[cardsNumber[i]]);
            for (int j = 0; j < cardsNumber[i]; j++) {
                allCards[0][j] = Instantiate(card);
                allCards[0][j].GetComponent<Card>().cardType = (Card.CardType)i;
                allCards[0][j].GetComponent<Card>().UpdateCard(cardMaterials[i]);
            }
        }

        drawPileManager.InitializeDrawPile(allCards);
    }
}
