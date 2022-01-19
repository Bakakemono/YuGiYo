using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotDistributor : MonoBehaviour {
    [SerializeField] private GameObject[] playerSlots = new GameObject[4];
    [SerializeField] private GameObject[] handSlots = new GameObject[4];
    [SerializeField] private GameObject[] fieldSlots = new GameObject[4];

    public GameObject GetPlayerSlots(int id) {
        return playerSlots[id];
    }

    public GameObject GetHandSlots(int id) {
        return handSlots[id];
    }

    public GameObject GetfieldSlots(int id) {
        return fieldSlots[id];
    }
}