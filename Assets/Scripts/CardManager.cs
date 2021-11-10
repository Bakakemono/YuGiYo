using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public enum CardType {
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
        BEER,
        Length,
        NONE
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

    public enum CardEndLocaion
    {

    }

    [SerializeField] GameObject card;

    [SerializeField]
    List<Material> cardMaterials = new List<Material>();

    DrawPileManager drawPileManager;

    [SerializeField] List<Player> players;

    int startingCardNumber = 6;

    bool startDistributingFirstHand = false;

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
    }

    private void Update() {
        if(drawPileManager.drawPileInstantiated && !startDistributingFirstHand) {
            startDistributingFirstHand = true;
            StartCoroutine(DrawInitialHand());
        }
    }

    public void InstantiateCards() {
        List<GameObject> allCards = new List<GameObject>();

        for (int i = 0; i < cardsNumber.Count; i++) {
            for (int j = 0; j < cardsNumber[i]; j++) {
                allCards.Add(Instantiate(card, Vector3.one * 1000.0f, Quaternion.identity));
                Card newCard = allCards[allCards.Count - 1].GetComponent<Card>();
                newCard.cardType = (CardType)i;
                newCard.ID = j;
                newCard.UpdateCard(cardMaterials[i]);
            }
        }

        drawPileManager.InitializeDrawPile(allCards);
    }

    void DrawCardToPlayer(Player player) {
        Vector2Int index = player.hand.AddCard(drawPileManager.DrawCard());
        player.handDisplayer.AddCardToDisplay(player.hand.cards[(CardType)index.x][index.y]);
    }

    IEnumerator DrawInitialHand() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < startingCardNumber; i++) {
            foreach (Player player in players) {
                DrawCardToPlayer(player);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }


}
