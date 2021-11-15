using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeTest : MonoBehaviour
{
    [SerializeField] int nmbSide = 16;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        //for (int i = 0; i < nmbSide; i++) {
        //    Gizmos.DrawLine(
        //        new Vector3());
        //}
    }
}
