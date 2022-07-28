using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DryadeEffect", menuName = "Cards Effects/Dryade")]
public class DryadeEffect : CardEffect {
    public override void Effect(Player _dealer, Player _target, Vector2[] _dealerCards, Vector2[] _targetCards, CardManager _cardManager) {
        for (int i = 0; i < _targetCards.Length; i++) {
            Card targetCard = _cardManager.GetCard(_targetCards[i]);

            _target.fieldManager.RemoveCardFormDisplay(targetCard);
            _target.field.RemoveCard(targetCard);
            _dealer.fieldManager.AddCardToField(targetCard);
            _dealer.field.AddCard(targetCard);
        }
    }
}
