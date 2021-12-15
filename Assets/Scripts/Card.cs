using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
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

    private const int INVALID_ID = -1;

    [SerializeField] public CardManager.CardType cardType = CardManager.CardType.NONE;

    [SerializeField] public int id = INVALID_ID;

    [SerializeField] private MeshRenderer faceRenderer;

    public Transform customTransform;

    private void Start() {
        customTransform = transform;
    }

    public void UpdateCard(Material cardMaterial) {
        faceRenderer.material = cardMaterial;
    }
}