using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeDrawer : MonoBehaviour {
    [SerializeField]
    int sideNumber = 16;

    const int quarter = 4;

    [SerializeField]
    float size = 2.0f;

    [Range(0.0f, 1.0f)] [SerializeField]
    float decal = 0.5f;
    float QuarterAngle = 90.0f;

    Transform obj;
    float roundTime = 3.0f;
    const int FRAME_PER_SEC = 50;
    int totalFrame;
    int frameCount = 0;

    [SerializeField] List<float> facesAngles = new List<float>();

    private void Start() {
        totalFrame = (int)roundTime * FRAME_PER_SEC;
    }

    void FixedUpdate() {
        //obj.position = new Vector3(
        //            Mathf.Sin((i + decal) / 16 / 360.0f * Mathf.Deg2Rad) * size,
        //            0.0f,
        //            Mathf.Cos((i + decal) / 16 / 360.0f * Mathf.Deg2Rad) * size);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        //for (float i = 0; i < sideNumber; i++) {
        //    Gizmos.DrawSphere(transform.position + new Vector3(
        //            Mathf.Sin((i + decal) / sideNumber / 360.0f * Mathf.Deg2Rad) * size,
        //            0.0f,
        //            Mathf.Cos((i + decal) / sideNumber / 360.0f * Mathf.Deg2Rad) * size),
        //            0.1f);

        //    Gizmos.DrawLine(
        //        transform.position + new Vector3(
        //            Mathf.Sin((i + decal) / sideNumber / 360.0f * Mathf.Deg2Rad) * size,
        //            0.0f,
        //            Mathf.Cos((i + decal) / sideNumber / 360.0f* Mathf.Deg2Rad) * size),
        //        transform.position + new Vector3(
        //            Mathf.Sin(((i + 1) % sideNumber + decal) / sideNumber / 360.0f * Mathf.Deg2Rad) * size,
        //            0.0f,
        //            Mathf.Cos(((i + 1) % sideNumber + decal) / sideNumber / 360.0f * Mathf.Deg2Rad) * size)
        //        );
        //}
        float angleAdded = 0.0f;
        for (int i = 0; i < sideNumber; i++) {
            Gizmos.DrawLine(
                transform.position + new Vector3(
                    Mathf.Sin((angleAdded + decal * QuarterAngle) * Mathf.Deg2Rad) * size,
                    0.0f,
                    Mathf.Cos((angleAdded + decal * QuarterAngle) * Mathf.Deg2Rad) * size),
                transform.position + new Vector3(
                    Mathf.Sin((angleAdded + decal * QuarterAngle + facesAngles[i % facesAngles.Count]) * Mathf.Deg2Rad) * size,
                    0.0f,
                    Mathf.Cos((angleAdded + decal * QuarterAngle + facesAngles[i % facesAngles.Count]) * Mathf.Deg2Rad) * size)
                );

            angleAdded += facesAngles[i % facesAngles.Count];
        }

        Gizmos.DrawLine(
            Vector3.right * size,
            Vector3.left * size
            );
    }
}
