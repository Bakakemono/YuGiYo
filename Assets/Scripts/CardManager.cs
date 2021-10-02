using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public enum CardType
    {
        HUMAN,
        WEREWOLF,
        OGRE,
        DRYAD,
        KORRIGAN,
        GNOME,
        FAIRY,
        LEPRECHAUN,
        FARFADET,
        ELF,
        HUMAN_SECRET,
        WEREWOLF_SECRET,
        OGRE_SECRET,
        DRYAD_SECRET,
        KORRIGAN_SECRET,
        GNOME_SECRET,
        FAIRY_SECRET,
        LEPRECHAUN_SECRET,
        FARFADET_SECRET,
        ELF_SECRET,
        BEER
    }

    [SerializeField]
    public List<int> cardsNumber = new List<int> {
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

    [SerializeField] GameObject card;

    [SerializeField]
    List<Material> cardMaterials = new List<Material>();

    DrawPileManager drawPileManager;

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
    }

    void InstantiateCards() {
        List<GameObject> allCards = new List<GameObject>();

        for (int i = 0; i < cardsNumber.Count; i++) {
            for (int j = 0; j < cardsNumber[i]; j++) {
                allCards.Add(Instantiate(card, Vector3.one * 1000.0f, Quaternion.identity));
                allCards[allCards.Count - 1].GetComponent<Card>().cardType = (CardType)i;
                allCards[allCards.Count - 1].GetComponent<Card>().UpdateCard(cardMaterials[i]);
            }
        }

        drawPileManager.InitializeDrawPile(allCards);
    }

    void InitialHand() {

    }
}
