using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotDistributor : MonoBehaviour {
    [SerializeField] private Transform[] playerSlots = new Transform[4];
    [SerializeField] private HandDisplayer[] handSlots = new HandDisplayer[4];
    [SerializeField] private FieldDisplayer[] fieldSlots = new FieldDisplayer[4];

    [SerializeField] private Transform showCasePosition;

    public Transform GetPlayerSlots(int id) {
        return playerSlots[id];
    }

    public HandDisplayer GetHandSlots(int id) {
        return handSlots[id];
    }

    public FieldDisplayer GetfieldSlots(int id) {
        return fieldSlots[id];
    }

    public Transform GetShowCasePosition() {
        return showCasePosition;
    }
}