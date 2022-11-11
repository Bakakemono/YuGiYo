using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DryadeEffect", menuName = "Cards Effects/Dryade")]
public class PeopleEffect_04_Dryade : CardEffect {
    public override void DoEffect(
        Player _dealer,
        Player _target,
        Vector2[] _dealerHandSelectedCards,
        Vector2[] _dealerFieldSelectedCards,
        Vector2[] _targetHandSelectedCards,
        Vector2[] _targetFieldSelectedCards,
        CardManager _cardManager) {

        for (int i = 0; i < _targetFieldSelectedCards.Length; i++) {
            Card targetCard = _cardManager.GetCard(_targetFieldSelectedCards[i]);

            _target.fieldManager.RemoveCard(targetCard);
            _target.GetField().RemoveCard(targetCard);
            _dealer.fieldManager.AddCard(targetCard);
            _dealer.GetField().AddCard(targetCard);
        }
    }
}
