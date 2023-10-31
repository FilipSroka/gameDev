using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;  // Required for Image component

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 7.0f;
    public Transform playerTransform;
    public TextMeshProUGUI deathText;
    public Image screenOverlay;  // UI Image for color shift effect

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private Vector3 startingPoint;
    private float baseSpeed;
    private float baseJumpForce;
    private int lives = 3;

    private float cocaine_effect = 0;
    private float weed_effect = 0;
    private bool isUnderEffect = false;
    private Transform currentRespawnPoint;

    // Delayed Input
    private float lastInputTime;

    private bool isUnderWeedEffect = false;
    private Queue<Action> inputBuffer = new Queue<Action>();
    private Color originalColor;
    private float weedDelay;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPoint = transform.position;
        deathText.text = "";
        baseSpeed = moveSpeed;
        baseJumpForce = jumpForce;
        originalColor = Camera.main.backgroundColor;
    }

    void Update()
    {
        if (isUnderWeedEffect)
        {
            // This creates a slight delay before processing input
            StartCoroutine(DelayedInputProcessing());
        }
        else
        {
            ProcessInput();
        }

        UpdateCocaineEffect();
        UpdateWeedEffect();
    }

    private IEnumerator DelayedInputProcessing()
    {
        float delay = 0.1f * (weed_effect / 100f);  // This will result in a maximum delay of 0.1 seconds at full weed effect
        yield return new WaitForSeconds(delay);
        ProcessInput();
    }

    private void ProcessInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            lives--;
            if (lives <= 0)
            {
                Die();
            }
            else
            {
                Respawn();
            }
        }
        else
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            currentRespawnPoint = other.transform;
        }
        else if (other.CompareTag("WaterWall"))
        {
            Die();
        }
        else if (other.CompareTag("Cocaine"))
        {
            cocaine_effect = 100;
            isUnderEffect = true;
        }
        else if (other.CompareTag("Weed"))
        {
            weed_effect = 100;
            isUnderWeedEffect = true;
        }
    }

    void Die()
    {
        float distanceTraveled = transform.position.x - startingPoint.x;
        deathText.text = "You died! Distance traveled: " + distanceTraveled.ToString("F2") + " units.";
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(2);
        transform.position = startingPoint;
        rb.velocity = Vector2.zero;
        deathText.text = "";
    }

    void Respawn()
    {
        if (currentRespawnPoint != null)
        {
            transform.position = currentRespawnPoint.position;
            rb.velocity = Vector2.zero;
        }
        else
        {
            transform.position = startingPoint;
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateCocaineEffect()
    {
        if (cocaine_effect > 0)
        {
            cocaine_effect -= 10 * Time.deltaTime;

            // Adjust speed and jump based on cocaine effect
            moveSpeed = baseSpeed * (1 + (cocaine_effect / 100f));
            jumpForce = baseJumpForce * (1 + 0.5f * (cocaine_effect / 100f));

            // Call camera shake
            StartCoroutine(CameraShake());

            if (cocaine_effect <= 0)
            {
                cocaine_effect = 0;
                isUnderEffect = false;
            }
        }
    }

    private void UpdateWeedEffect()
    {
        if (isUnderWeedEffect)
        {
            weed_effect -= 10 * Time.deltaTime;
            moveSpeed = baseSpeed * (1 - 0.2f * (weed_effect / 100f));
            jumpForce = baseJumpForce * (1 - 0.2f * (weed_effect / 100f));

            weedDelay = 0.1f * (weed_effect / 100f);
            StartCoroutine(TemporalColorShift());

            if (weed_effect <= 0)
            {
                weed_effect = 0;
                isUnderWeedEffect = false;
            }
        }
    }


    private IEnumerator CameraShake()
    {
        float elapsed = 0.0f;
        float duration = 0.5f;  // Fixed duration for shake effect
        float maxShakeMagnitude = 0.1f;  // Adjust this value to control maximum shake magnitude

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float magnitude = Mathf.Lerp(0, maxShakeMagnitude, cocaine_effect / 100f);  // Interpolate based on cocaine_effect

            // Get the camera's desired position, based on the player's position
            Vector3 playerPosition = new Vector3(playerTransform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            
            // Apply the shake effect based on the magnitude
            Vector3 offset = UnityEngine.Random.insideUnitSphere * magnitude;
            Camera.main.transform.position = playerPosition + offset;
            
            yield return null;
        }

        // Ensure the camera continues following the player after the shake effect ends
        Camera.main.transform.position = new Vector3(playerTransform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    private IEnumerator HandleBufferedInputs()
    {
        yield return new WaitForSeconds(weedDelay);
        while (inputBuffer.Count > 0)
        {
            var bufferedInput = inputBuffer.Dequeue();
            bufferedInput.Invoke();
        }
    }

    private IEnumerator TemporalColorShift()
    {
        while (weed_effect > 0)
        {
            float intensity = weed_effect / 100f;  // Normalize effect between 0 and 1
            Camera.main.backgroundColor = Color.Lerp(originalColor, new Color(0.5f, 0.5f, 0.7f), intensity);
            yield return null;  // Wait until next frame to adjust color
        }
        Camera.main.backgroundColor = originalColor;
    }

}