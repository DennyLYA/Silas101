using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform cameraParentTransform;

    private Vector3 newCameraParentPosition;

    public  float moveDuration;
    private float elaspedTime;

    // Start is called before the first frame update
    void Start()
    {
        newCameraParentPosition = cameraParentTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (elaspedTime >= 0f)
        {
            // Calculates the progress from start time to end
            elaspedTime += Time.deltaTime;
            float moveProgress = elaspedTime / moveDuration;

            // Smoothly moves the camera to the new position
            cameraParentTransform.position = Vector3.Lerp(cameraParentTransform.position, newCameraParentPosition, Mathf.SmoothStep(0, 1, moveProgress));
        }
    }

    // Moves the position of the Camera
    public void MovePosition()
    {
        // Moves the camera up
        newCameraParentPosition += Vector3.up;

        // reset the camera movement timer
        elaspedTime = 0;
    }
}
