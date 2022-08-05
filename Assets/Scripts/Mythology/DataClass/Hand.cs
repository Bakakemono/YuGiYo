using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public Dictionary<CardManager.CardType, List<Card>> cards = new Dictionary<CardManager.CardType, List<Card>>();
    public List<Card> unorderedCards = new List<Card>();

    public Vector2Int AddCard(Card _card) {
        unorderedCards.Add(_card);
        if (cards.ContainsKey(_card.GetCardType())) {
            cards[_card.GetCardType()].Add(_card);
            return new Vector2Int((int)_card.GetCardType(), cards[_card.GetCardType()].Count - 1);
        }
        else {
            List<Card> newCardType = new List<Card>();
            newCardType.Add(_card);
            cards.Add(_card.GetCardType(), newCardType);
            return new Vector2Int((int)_card.GetCardType(), cards[_card.GetCardType()].Count - 1);
        }
    }

    public void RemoveCard(Card _card) {
        if (!cards.ContainsKey(_card.GetCardType())) {
            return;
        }
        cards[_card.GetCardType()].Remove(_card);
        if (cards[_card.GetCardType()].Count == 0) {
            cards.Remove(_card.GetCardType());
        }
        unorderedCards.Remove(_card);
    }

    public bool IsCardInHand(Card _card) {
        if (cards.ContainsKey(_card.GetCardType())) {
            return cards[_card.GetCardType()].Contains(_card);
        }

        return false;
    }

    public bool IsEmpty() {
        return cards.Count == 0;
    }

    public void ResetHand() {
        cards.Clear();

        unorderedCards.Clear();
    }
}
