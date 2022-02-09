using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    [SerializeField] private bool selectPlayer = false;
    [SerializeField] private bool targetHand = false;
    [SerializeField] private bool targetField = false;
    [SerializeField] private bool selectCard = false;

    public virtual void Effect(Player _dealer, Player _target, CardManager _cardManager) {}

    public bool SelectPlayer() {
        return selectCard;
    }
    public bool TargetHand() {
        return targetHand;
    }

    public bool TargetField() {
        return targetField;
    }

    public bool SelectCard() {
        return selectCard;
    }
}
