using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotDistributor : MonoBehaviour {
    [SerializeField] private Transform[] playerSlots = new Transform[4];
    [SerializeField] private HandManager[] handSlots = new HandManager[4];
    [SerializeField] private FieldManager[] fieldSlots = new FieldManager[4];

    [SerializeField] private Transform showCasePosition;

    public Transform GetPlayerSlots(int id) {
        return playerSlots[id];
    }

    public HandManager GetHandSlots(int id) {
        return handSlots[id];
    }

    public FieldManager GetfieldSlots(int id) {
        return fieldSlots[id];
    }

    public Transform GetShowCasePosition() {
        return showCasePosition;
    }
}