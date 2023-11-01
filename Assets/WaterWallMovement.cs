using UnityEngine;

public class WaterWallMovement : MonoBehaviour
{
    public float initialSpeed = 2.0f;
    public float acceleration = 0.01f;
    private float currentSpeed;

    void Start()
    {
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        // Move the water wall
        transform.position += Vector3.right * currentSpeed * Time.deltaTime;

        // Gradually increase the speed
        currentSpeed += acceleration * Time.deltaTime;
    }
}
