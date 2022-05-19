using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform customTransform;

    Vector3 oldPosition = new Vector3();
    Quaternion oldRotation = new Quaternion();

    Vector3 newPosition = new Vector3();
    Quaternion newRotation = new Quaternion();

    // Number of frame the camera will take to move
    float totalStepNumber = 50;

    // Current step the camera is at
    float currentStep = 0;

    // Flag for when the camera position need to be updated
    bool needUpdate = false;

    [SerializeField] Transform mainPlayerTransform;

    // Positions and rotations for card in hand selection
    [SerializeField] Transform[] playerToHandTransform;

    // Position and rotations for card in field selection
    [SerializeField] Transform[] playerToFieldTransform;

    private void Start()
    {
        customTransform = transform;
    }

    private void FixedUpdate()
    {
        if (!needUpdate)
            return;


        currentStep++;
        customTransform.position = Vector3.Lerp(oldPosition, newPosition, currentStep / totalStepNumber);
        customTransform.rotation = Quaternion.Lerp(oldRotation, newRotation, currentStep / totalStepNumber);
        
        // Check if the camera finish moving
        if(currentStep >= totalStepNumber)
        {
            currentStep = 0;
            needUpdate = false;
        }
    }

    public void LookAtHand(int playerId) {
        oldPosition = customTransform.position;
        oldRotation = customTransform.rotation;

        newPosition = playerToHandTransform[playerId - 1].position;
        newRotation = playerToHandTransform[playerId - 1].rotation;

        needUpdate = true;
    }

    public void LookAtField(int playerId) {
        oldPosition = customTransform.position;
        oldRotation = customTransform.rotation;

        newPosition = playerToFieldTransform[playerId - 1].position;
        newRotation = playerToFieldTransform[playerId - 1].rotation;

        needUpdate = true;
    }

    public void ResetCamera() {
        oldPosition = customTransform.position;
        oldRotation = customTransform.rotation;

        newPosition = mainPlayerTransform.position;
        newRotation = mainPlayerTransform.rotation;

        needUpdate = true;
    }
}
