using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
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
        BEER                
    }

    public static int cardColumnNmb = 6;
    public static int cardRawNmb = 4;
    public static float cardLength = 2.54f;
    public static float cardWidth = 1.778f;
    public static int raceNmb = 10;

    [SerializeField] public CardManager.CardType cardType = CardManager.CardType.NONE;

    [SerializeField] private MeshRenderer faceRenderer;

    public Transform customTransform;

    private void Start() {
        customTransform = transform;
    }

    public void UpdateCard(Material cardMaterial) {
        faceRenderer.material = cardMaterial;
    }
}

[System.Serializable]
public struct Hand
{
    public Dictionary<CardManager.CardType, List<Card>> cards;

    public Hand(int a = 0) {
        cards = new Dictionary<CardManager.CardType, List<Card>>();
    }

    public Vector2Int AddCard(Card card) {
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
        cards[card.cardType].Remove(card);
        if (cards[card.cardType].Count == 0) {
            cards.Remove(card.cardType);
        }
    }

    public bool IsCardInHand(Card card) {

        if (cards.ContainsKey(card.cardType)) {
            return cards[card.cardType].Contains(card);
        }

        return false;
    }
}