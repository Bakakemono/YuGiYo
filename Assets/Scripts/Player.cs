using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Hand hand;
    public List<Card> unorderedCard = new List<Card>();

    private void Start() {
        hand = new Hand(0);
    }

    private void Update() {
        
    }
}
