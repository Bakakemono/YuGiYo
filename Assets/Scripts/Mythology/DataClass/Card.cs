using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public static int cardColumnNmb = 6;
    public static int cardRawNmb = 4;
    public static float cardLength = 0.15f * 6.0f;
    public static float cardWidth = 0.08f * 6.0f;
    public static float cardThickness = 0.003f * 6.0f;

    private const int INVALID_ID = -1;

    [SerializeField] private CardManager.CardType cardType = CardManager.CardType.NONE;

    [SerializeField] private int id = INVALID_ID;

    [SerializeField] private MeshRenderer faceRenderer;

    private Transform customTransform;

    private CardEffect cardEffect;

    private void Awake() {
        customTransform = transform;
    }

    public void UpdateCard(Material _cardMaterial) {
        faceRenderer.material = _cardMaterial;

        gameObject.name = cardType.ToString() + " " + id.ToString();
    }

    public CardManager.CardType GetCardType() {
        return cardType;
    }

    public void SetCardType(CardManager.CardType _cardType) {
        cardType = _cardType;
    }

    public int GetId() {
        return id;
    }

    public void SetId(int _id) {
        id = _id;
    }

    public Transform GetTransform() {
        return transform;
    }

    public CardEffect GetCardEffect() {
        return cardEffect;
    }

    public void SetCardEffect(CardEffect _cardEffect) {
        cardEffect = _cardEffect;
    }
}