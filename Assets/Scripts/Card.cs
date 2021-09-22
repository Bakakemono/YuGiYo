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

    [SerializeField] public CardType cardType = CardType.HUMAN;

    public void UpdateCard(Material cardMaterial) {
        //Material cardMaterial = GetComponent<MeshRenderer>().material;

        //cardMaterial.SetTextureOffset(
        //    0,
        //    new Vector2((int)cardType % cardColumnNmb * cardColumnDecal,
        //    Mathf.FloorToInt((float)cardType / cardRawNmb) * cardRawDecal));

        GetComponent<MeshRenderer>().material = cardMaterial;
    }
}
