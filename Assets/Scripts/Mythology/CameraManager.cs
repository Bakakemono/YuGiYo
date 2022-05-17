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

    float totalStepNumber = 50;
    float currentStep = 0;

    bool needUpdated = false;

    [SerializeField] Transform[] playerLocations; 

    private void Start()
    {
        customTransform = transform;
    }

    private void FixedUpdate()
    {
        if (!needUpdated)
            return;

        currentStep++;
        customTransform.position = Vector3.Lerp(oldPosition, newPosition, currentStep / totalStepNumber);
        customTransform.rotation = Quaternion.Lerp(oldRotation, newRotation, currentStep / totalStepNumber);
        
        if(currentStep == totalStepNumber)
        {
            currentStep = 0;
            needUpdated = false;
        }
    }

    public void GoToPlayer(int playerId)
    {
        MoveCamera(playerLocations[playerId]);
    }

    private void MoveCamera(Transform _newLocation)
    {
        oldPosition = customTransform.position;
        oldRotation = customTransform.rotation;
        newPosition = _newLocation.position;
        newRotation = _newLocation.rotation;

        needUpdated = true;
    }
}
