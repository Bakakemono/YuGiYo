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
        FIELD,
        DISCARD_PILE
    }

    public enum ProgressCardPlayed
    {
        WAITING_FOR_CARD,
        CARD_PLAYED,
        RESET
    }

    ProgressCardPlayed progressCardPlayed = ProgressCardPlayed.WAITING_FOR_CARD;

    [SerializeField] GameObject card;

    [SerializeField]
    List<Material> cardMaterials = new List<Material>();

    GameManager gameManager;
    DrawPileManager drawPileManager;
    DiscardPileManager discardPileManager;

    [SerializeField] List<Player> players;

    [SerializeField] int startingCardNumber = 6;

    bool startDistributingFirstHand = false;

    public bool initialHandGiven = false;

    [SerializeField] Transform cardShowcasePosition;

    Card cardPlayed = null;
    Player player = null;

    float lerpSpeed = 0.05f;
    float distanceTolerance = 0.1f;

    void Start() {
        drawPileManager = FindObjectOfType<DrawPileManager>();
        discardPileManager = FindObjectOfType<DiscardPileManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate() {
        if(drawPileManager.drawPileInstantiated && !startDistributingFirstHand) {
            startDistributingFirstHand = true;
            StartCoroutine(DrawInitialHand());
        }
        if (initialHandGiven) {
            CardPlayedProgress();
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

    public void DrawCardToPlayer(Player player) {
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
        yield return new WaitForSeconds(4.0f);
        initialHandGiven = true;
    }

    public CardEndLocaion CardLocaionGoal(CardType cardType) {
        switch (cardType) {
            case CardType.BEER:
                return CardEndLocaion.DISCARD_PILE;

            default:
                return CardEndLocaion.FIELD;
        }
    }

    public void PlayCard(Player pl, Card cd) {
        player = pl;
        cardPlayed = cd;
    }

    public void CardPlayedProgress() {
        switch (progressCardPlayed) {
            case ProgressCardPlayed.WAITING_FOR_CARD:
                if(player != null && cardPlayed != null) {
                    progressCardPlayed = ProgressCardPlayed.CARD_PLAYED;
                }
                break;
            case ProgressCardPlayed.CARD_PLAYED:
                player.canPlay = false;
                cardPlayed.customTransform.parent = cardShowcasePosition.transform;
                cardPlayed.customTransform.localPosition = Vector3.Lerp(cardPlayed.customTransform.localPosition, Vector3.zero, lerpSpeed);
                cardPlayed.customTransform.localRotation = Quaternion.Lerp(cardPlayed.customTransform.localRotation, Quaternion.identity, lerpSpeed);
                if (Vector3.SqrMagnitude(cardPlayed.customTransform.localPosition) < distanceTolerance * distanceTolerance) {
                    if(CardLocaionGoal(cardPlayed.cardType) == CardEndLocaion.DISCARD_PILE) {
                        player.hand.RemoveCard(cardPlayed);
                        discardPileManager.DiscardCard(cardPlayed);
                        player.canPlay = true;
                    }
                    else {
                        player.hand.RemoveCard(cardPlayed);
                        player.field.AddCard(cardPlayed);
                        player.fieldDisplayer.AddCardToField(cardPlayed);
                        player.canPlay = true;
                        gameManager.NextPlayer();
                    }
                    progressCardPlayed = ProgressCardPlayed.RESET;
                }
                break;
            case ProgressCardPlayed.RESET:
                player = null;
                cardPlayed = null;
                progressCardPlayed = ProgressCardPlayed.WAITING_FOR_CARD;
                break;
        }
    }
}