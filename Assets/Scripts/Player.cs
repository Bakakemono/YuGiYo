using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour {
    public Hand hand;

    private void Start() {
        hand = new Hand(0);
    }


}
