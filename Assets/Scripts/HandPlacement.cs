using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class HandPlacement : MonoBehaviour
{
    [SerializeField] Transform mainPlayerHand;

    [SerializeField] Transform handPos;

    bool update = false;

    private void FixedUpdate() {
        if (!update)
            return;

        mainPlayerHand.position = handPos.position;
        mainPlayerHand.rotation = handPos.rotation;
    }
}
