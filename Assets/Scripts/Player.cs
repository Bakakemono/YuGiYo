using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour {
    public Hand hand;
    public List<Card> unorderedCard = new List<Card>();

    private void Start() {
        hand = new Hand(0);
    }

    private void Update() {
        unorderedCard = new List<Card>();
        CardManager.CardType lastTypeChecked = (CardManager.CardType)0;
        for (int i = 0; i < hand.cards.Count; i++) {
            for (CardManager.CardType j = 0; j < CardManager.CardType.Length; j++) {
                if (hand.cards.ContainsKey(j)) {

                    int totalCard = hand.cards[j].Count;
                    for (int z = 0; z < totalCard; z++) {
                        unorderedCard.Add(hand.cards[j][z]);
                    }
                    lastTypeChecked++;
                    break;
                }
            }
        }
    }
}
