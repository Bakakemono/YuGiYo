using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SecretEffect", menuName = "Cards Effects/Secret")]
public class SecretEffect : CardEffect
{
    public override void Effect(Player _dealer, Player _target, CardManager _cardManager) {
        return;
    }
}
