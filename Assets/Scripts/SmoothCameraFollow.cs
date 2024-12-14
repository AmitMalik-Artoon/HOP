using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Adjust for smoothness
    public Vector3 offset; // Offset to maintain a specific distance from the player

    void LateUpdate()
    {
        // Define the target position based on the player's position and offset
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z) + offset;

        // Smoothly move the camera to the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Assign the new position to the camera
        transform.position = smoothedPosition;
    }
}
