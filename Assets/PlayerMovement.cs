using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Add this line
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;  // Required for Image component

public class PlayerMovement : MonoBehaviour
{
    private class DrugEffectInstance
    {
        public float timeElapsed;
        public float peakTime;
        public float totalDuration;
    }
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
    public CircleManager circleManager;

    private float cocaine_effect = 0;
    private float weed_effect = 0;
    private float mushroom_effect = 0;
    private bool isUnderCocaineEffect = false;
    private bool isUnderMushroomEffect = false;
    private Transform currentRespawnPoint;

    private Dictionary<string, List<DrugEffectInstance>> drugEffects = new Dictionary<string, List<DrugEffectInstance>>();

    // Delayed Input
    private float lastInputTime;

    private bool isUnderWeedEffect = false;
    private Queue<Action> inputBuffer = new Queue<Action>();
    private Color originalColor;
    private float weedDelay;

    private float maxDistance = 0;

    private bool current_color_routine = false; // Add this flag

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
        if (weed_effect > 0)
        {
            // This creates a slight delay before processing input
            StartCoroutine(DelayedInputProcessing());
        }
        else
        {
            ProcessInput();
        }

        UpdateDrugEffects();


        Debug.Log("Cocaine Effect: " + cocaine_effect);
        Debug.Log("Weed Effect: " + weed_effect);
        Debug.Log("Mushroom Effect: " + mushroom_effect);
    }

    private IEnumerator DelayedInputProcessing()
    {
        float delay = 1f * (weed_effect / 100f);  // This will result in a maximum delay of 0.1 seconds at full weed effect
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
        if (collision.gameObject.CompareTag("WaterDrop") || collision.gameObject.CompareTag("Water"))
        {
            float distanceTraveled = transform.position.x - startingPoint.x;
            if (distanceTraveled > maxDistance)
            {
                maxDistance = distanceTraveled;
            }
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
        if (other.CompareTag("Cocaine") || other.CompareTag("Weed") || other.CompareTag("Mushroom"))
                {
                    ApplyDrugEffect(other.tag);
                }
    }

    /// DRUGS

    private void ApplyDrugEffect(string drug)
    {
        float peakTime = 0;
        float totalDuration = 0;

        switch (drug)
        {
            case "Cocaine":
                peakTime = 10;
                totalDuration = 30;
                break;
            case "Weed":
                peakTime = 20;
                totalDuration = 40;
                break;
            case "Mushroom":
                peakTime = 30;
                totalDuration = 50;
                break;
        }

        if (!drugEffects.ContainsKey(drug))
        {
            drugEffects[drug] = new List<DrugEffectInstance>();
        }

        drugEffects[drug].Add(new DrugEffectInstance { timeElapsed = 0, peakTime = peakTime, totalDuration = totalDuration });
    }

    private void UpdateDrugEffects()
    {
        foreach (var drug in drugEffects.Keys)
        {
            float totalEffect = 0;
            var effectsList = drugEffects[drug];

            for (int i = effectsList.Count - 1; i >= 0; i--)
            {
                DrugEffectInstance effect = effectsList[i];
                effect.timeElapsed += Time.deltaTime;

                float x = effect.timeElapsed;
                float mu = effect.peakTime;
                float sigma = (effect.totalDuration - effect.peakTime) / 3;  // Assuming sigma is 1/3 of the duration after peak

                // Amplitude is set to 50
                float A = 50f;

                float drugEffect = A * Mathf.Exp(-Mathf.Pow(x - mu, 2) / (2 * Mathf.Pow(sigma, 2)));
                totalEffect += drugEffect;

                if (effect.timeElapsed >= effect.totalDuration)
                {
                    effectsList.RemoveAt(i);
                    continue;
                }
            }

            // Apply effects based on the drug
            if (drug == "Cocaine")
            {
                cocaine_effect = totalEffect;
                moveSpeed = baseSpeed * (1 + (cocaine_effect / 100f));
                jumpForce = baseJumpForce * (1 + 0.5f * (cocaine_effect / 100f));
                StartCoroutine(CameraShake(cocaine_effect));
            }
            else if (drug == "Weed")
            {
                weed_effect = totalEffect;
                moveSpeed = baseSpeed * (1 - 0.2f * (weed_effect / 100f));
                jumpForce = baseJumpForce * (1 - 0.2f * (weed_effect / 100f));
                if (weed_effect > 0 && !current_color_routine)
                {
                    StartCoroutine(TemporalColorShift());
                }
            }
            else if (drug == "Mushroom")
            {
                mushroom_effect = totalEffect;
                circleManager.SpawnCirclesWithIntensity(mushroom_effect);
            }
        }
    }

    void Die()
    {
        deathText.text = "You died! Distance traveled: " + maxDistance.ToString("F2") + " units.";
        deathText.fontSize = 72; 
        RectTransform rt = deathText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600, 200);  // Width: 600, Height: 200
        // Position the text in the center
        rt.anchoredPosition = Vector3.zero;
        StartCoroutine(RestartGameAfterDelay());
    
    }

    IEnumerator RestartGameAfterDelay()
    {
        // Disable player movement and other gameplay elements here
        // For example, set a flag or disable a script/component

        // Example:
        // this.enabled = false;  // Disables this script

        yield return new WaitForSeconds(5);  // Wait for 5 seconds

        // Restart the game by reloading the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            StartCoroutine(CameraShake(cocaine_effect));

            if (cocaine_effect <= 0)
            {
                cocaine_effect = 0;
                isUnderCocaineEffect = false;
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

    private void UpdateMushroomEffect()
    {
        if (isUnderMushroomEffect)
        {
            mushroom_effect -= 10 * Time.deltaTime;
            circleManager.SpawnCirclesWithIntensity(mushroom_effect);

            if (mushroom_effect <= 0)
            {
                mushroom_effect = 0;
                isUnderMushroomEffect = false;
            }
        }
    }

    private IEnumerator CameraShake(float effect)
    {
        float elapsed = 0.0f;
        float duration = 0.5f;  // Fixed duration for shake effect
        float maxShakeMagnitude = 0.05f;  // Adjust this value to control maximum shake magnitude

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float magnitude = Mathf.Lerp(0, maxShakeMagnitude, effect / 100f);  // Interpolate based on cocaine_effect

            // Get the camera's desired position, based on the player's position
            Vector3 playerPosition = new Vector3(playerTransform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            
            // Apply the shake effect based on the magnitude
            Vector3 offset = UnityEngine.Random.insideUnitSphere * magnitude;
            offset.z = 0;
            Camera.main.transform.position = playerPosition + offset;
            
            yield return null;
        }

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
        current_color_routine = true;  // Set the flag to true

        while (weed_effect > 0)
        {
            float intensity = weed_effect / 100f;
            float transitionSpeed = Mathf.Lerp(0.2f, 5f, intensity);
            Color targetColor = new Color(
                UnityEngine.Random.Range(0.0f, 1.0f),
                UnityEngine.Random.Range(0.0f, 1.0f),
                UnityEngine.Random.Range(0.0f, 1.0f)
            );
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, targetColor, transitionSpeed * Time.deltaTime);
            yield return null;
        }

        current_color_routine = false;  // Reset the flag when coroutine ends
    }

}