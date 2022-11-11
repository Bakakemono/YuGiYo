using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffect : ScriptableObject {
    [SerializeField] public int nmbCardTargeted = 0;

    [SerializeField] public CardManager.CardSelectionStep[] cardSelectionSteps;


    public virtual void DoEffect(
        Player _dealer, 
        Player _target, 
        Vector2[] _dealerHandSelectedCards,
        Vector2[] _dealerFieldSelectedCards,
        Vector2[] _targetHandSelectedCards,
        Vector2[] _targetFieldSelectedCards,
        CardManager _cardManager) { }


    public int GetTargetNumber() {
        return nmbCardTargeted;
    }

    public CardManager.CardSelectionStep[] GetCardSelectionSteps() {
        return cardSelectionSteps;
    }
}