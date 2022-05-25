using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public Dictionary<CardManager.CardType, List<Card>> cards = new Dictionary<CardManager.CardType, List<Card>>();
    public List<Card> unorderedCards = new List<Card>();

    public Vector2Int AddCard(Card card) {
        unorderedCards.Add(card);
        if (cards.ContainsKey(card.cardType)) {
            cards[card.cardType].Add(card);
            return new Vector2Int((int)card.cardType, cards[card.cardType].Count - 1);
        }
        else {
            List<Card> newCardType = new List<Card>();
            newCardType.Add(card);
            cards.Add(card.cardType, newCardType);
            return new Vector2Int((int)card.cardType, cards[card.cardType].Count - 1);
        }
    }

    public void RemoveCard(Card card) {
        if (!cards.ContainsKey(card.cardType)) {
            return;
        }
        cards[card.cardType].Remove(card);
        if (cards[card.cardType].Count == 0) {
            cards.Remove(card.cardType);
        }
        unorderedCards.Remove(card);
    }

    public bool IsCardInHand(Card card) {
        if (cards.ContainsKey(card.cardType)) {
            return cards[card.cardType].Contains(card);
        }

        return false;
    }

    public bool IsEmpty() {
        return cards.Count == 0;
    }
}
