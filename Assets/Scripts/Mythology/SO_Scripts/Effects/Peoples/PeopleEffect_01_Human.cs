using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanEffect", menuName = "Cards Effects/Human")]
public class PeopleEffect_01_Human : CardEffect {
    public override void DoEffect(
        Player _dealer,
        Player _target,
        Vector2[] _dealerHandSelectedCards,
        Vector2[] _dealerFieldSelectedCards,
        Vector2[] _targetHandSelectedCards,
        Vector2[] _targetFieldSelectedCards,
        CardManager _cardManager) {
    }
}
