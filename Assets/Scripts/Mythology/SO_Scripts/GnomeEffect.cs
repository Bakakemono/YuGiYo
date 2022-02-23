using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GnomeEffect", menuName = "Cards Effects/Gnome")]
public class GnomeEffect : CardEffect
{
    [SerializeField] private int drawCardNumber = 2;
    [SerializeField] private float secondBetweenCardsDrawn = 0.4f;

    public override void Effect(Player _dealer, Player _target, Vector2 _dealerCard, Vector2 _targetCard, CardManager _cardManager) {
        _cardManager.DrawCardToPlayer(_dealer, drawCardNumber, secondBetweenCardsDrawn);
    }
}
