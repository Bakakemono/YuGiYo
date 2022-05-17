using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    [SerializeField] private bool targetPlayer = false;

    [SerializeField] private bool targetHand = false;
    [SerializeField] private bool targetField = false;

    [SerializeField] private float nmbCardTargeted = 0;


    [SerializeField] private bool selectHand = false;
    [SerializeField] private bool selectField = false;

    [SerializeField] private float nmbCardSelected = 0;


    public virtual void Effect(Player _dealer, Player _target, Vector2[] _dealerCards, Vector2[] _targetCards, CardManager _cardManager) {}

    public bool DoTargetPlayer() {
        return targetPlayer;
    }
    public bool DoTargetHand() {
        return targetHand;
    }

    public bool DoTargetField() {
        return targetField;
    }

    public bool DoSelectCard() {
        return selectHand;
    }

    public bool DoSelectField() {
        return selectField;
    }

    public float GetNmbCardTargeted()
    {
        return nmbCardTargeted;
    }
}
