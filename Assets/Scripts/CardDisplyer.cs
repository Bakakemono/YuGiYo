using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplyer : MonoBehaviour
{
    float cardNumber;
    [SerializeField] List<Transform> cards = new List<Transform>();
    [SerializeField] float maxAngle;
    [SerializeField] float fanningMaxAngle;

    [SerializeField] Vector3 fanPosition = Vector3.zero;
    [SerializeField] float circleRadius = 10.0f;
    public Vector3 circlePosition;

    List<Vector3> cardPositions = new List<Vector3>();

    public float maxAngleRadiant;
    public float fanningMaxAngleRadiant;

    void Start() {
        cardNumber = cards.Count;
        foreach (Transform card in cards) {
            cardPositions.Add(Vector3.zero);
        }

        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;
    }

    private void FixedUpdate() {
        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);

        maxAngleRadiant = maxAngle * Mathf.Deg2Rad;
        fanningMaxAngleRadiant = fanningMaxAngle * Mathf.Deg2Rad;

        float anglePerCard = fanningMaxAngleRadiant / (cardNumber - 1) < maxAngleRadiant ? fanningMaxAngleRadiant / (cardNumber - 1) : maxAngleRadiant;

        for(int i = 0; i < cardNumber; i++) {
            cardPositions[i] = new Vector3(
                circlePosition.x + circleRadius * Mathf.Sin((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f),
                circlePosition.y + circleRadius * Mathf.Cos((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f));

            cards[i].localPosition = cardPositions[i];
            cards[i].localRotation = Quaternion.Euler(new Vector3(0.0f, -10.0f, -((anglePerCard * i) - anglePerCard * (cardNumber - 1.0f) / 2.0f) * Mathf.Rad2Deg));
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        maxAngleRadiant = maxAngle / 360.0f * 2 * Mathf.PI;
        circlePosition = fanPosition - new Vector3(0.0f, circleRadius, 0.0f);


        Gizmos.DrawSphere(circlePosition, 0.3f);

        Gizmos.DrawLine(
            circlePosition,
                new Vector3(
                    circlePosition.x + circleRadius * Mathf.Sin(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
                    circlePosition.y + circleRadius * Mathf.Cos(-fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
            );

        Gizmos.DrawLine(
            circlePosition,
                new Vector3(
                    circlePosition.x + circleRadius * Mathf.Sin(fanningMaxAngle * Mathf.Deg2Rad / 2.0f),
                    circlePosition.y + circleRadius * Mathf.Cos(fanningMaxAngle * Mathf.Deg2Rad / 2.0f))
           );
    }
}
