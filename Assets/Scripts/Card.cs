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
    public static float cardColumnDecal = 1.555f;
    public static float cardRawDecal = -0.223f;

    [SerializeField] public CardManager.CardType cardType = CardManager.CardType.HUMAN;

    [SerializeField] private MeshRenderer faceRenderer;

    public Transform customTransform;

    private void Start() {
        customTransform = transform;
    }

    public void UpdateCard(Material cardMaterial) {
        faceRenderer.material = cardMaterial;
    }
}

struct Hand {
    public Dictionary<CardManager.CardType, List<Card>> cards;

    public Hand(int a = 0) {
        cards = new Dictionary<CardManager.CardType, List<Card>>();
    }
}