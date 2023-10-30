using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Adjust the speed as needed
    public float jumpForce = 7.0f; // Adjust the jump force as needed
    private Rigidbody2D rb;
    private bool isGrounded = true; // Initially assume the cube is grounded
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate the velocity for left and right movement
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Apply the velocity to the Rigidbody2D
        rb.velocity = movement;

        // Check for the jump input (Space key) and that the cube is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        // Apply an upward force to make the cube jump
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Set isGrounded to false to prevent multiple jumps
        isGrounded = false;
    }

    // Detect ground collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        if (collision.gameObject.CompareTag("WaterDrop"))
        {
            // Handle the collision as needed, such as triggering a game over state.
            Debug.Log("Player collided with the water drop zone.");
        }
    }

}
