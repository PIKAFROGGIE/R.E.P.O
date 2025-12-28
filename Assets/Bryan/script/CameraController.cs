using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Auto-Setup")]
    public bool calculateOffsetOnStart = true;

    void Start()
    {
        // If true, the script locks the camera to the distance set in the Scene View
        // automatically when you hit Play.
        if (calculateOffsetOnStart && player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Calculate where the camera WANTS to be
        Vector3 desiredPosition = player.position + offset;

        // 2. Smoothly interpolate between current position and desired position
        // This creates that nice "drag" effect rather than a hard lock
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Apply the position
        transform.position = smoothedPosition;
    }
}
