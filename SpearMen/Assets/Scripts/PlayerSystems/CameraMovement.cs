using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Camera Controls")]
    public Vector2 sensitivity = new Vector2(100f, 100f);
    public float distance = 5.0f;
    public Vector2 pitchLimits = new Vector2(-30f, 60f);
    public float cameraHeight = 2.0f;

    [Header("Collision Settings")]
    public LayerMask collisionMask; 
    public float collisionRadius = 0.3f; 
    public float collisionOffset = 0.2f; 

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity.x * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity.y * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        RotatePlayer(mouseX);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 targetPosition = target.position + Vector3.up * cameraHeight;

        Vector3 desiredCameraPos = targetPosition - (rotation * Vector3.forward * distance);

        if (Physics.SphereCast(
            targetPosition,
            collisionRadius,
            (desiredCameraPos - targetPosition).normalized,
            out RaycastHit hit,
            distance,
            collisionMask))
        {
            float adjustedDistance = hit.distance - collisionOffset;
            adjustedDistance = Mathf.Max(adjustedDistance, 0.3f);

            transform.position = targetPosition - (rotation * Vector3.forward * adjustedDistance);
        }
        else
        {
            transform.position = desiredCameraPos;
        }

        transform.rotation = rotation;
    }

    private void RotatePlayer(float mouseX)
    {
        target.Rotate(Vector3.up * mouseX);
    }
}
