using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FarfadetEffect", menuName = "Cards Effects/Farfadet")]
public class FarfadetEffect : CardEffect {
    public override void Effect(Player _dealer, Player _target, Vector2 _dealerCard, Vector2 _targetCard, CardManager _cardManager) {
        base.Effect(_dealer, _target, _dealerCard, _targetCard, _cardManager);
    }
}
