using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothing = 5f;
    public Vector2 offset;

    void Update()
    {
        // Ensure we have a player reference
        if (player == null)
            return;

        // Calculate the target position based on the player's position
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y) + offset;

        // Smoothly move the camera towards the target position
        Vector2 smoothedPosition = Vector2.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        // Assign the new position using a Vector3
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
