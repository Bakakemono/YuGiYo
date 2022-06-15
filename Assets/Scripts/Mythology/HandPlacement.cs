using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class HandPlacement : MonoBehaviour
{
    [SerializeField] Transform mainPlayerHand;

    [SerializeField] Transform handPos;

    bool update = false;

    private void FixedUpdate() {
        if (!update)
            return;

        UpdateHand();
    }

    private void OnGUI() {
        if(GUI.Button(new Rect(10, 10, 150, 100), "Update")) {
            UpdateHand();
        }
    }

    void UpdateHand() {
        mainPlayerHand.position = handPos.position;
        mainPlayerHand.rotation = handPos.rotation;
    }
}
