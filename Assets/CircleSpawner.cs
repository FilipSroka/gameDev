// CircleSpawner.cs
using UnityEngine;
using System.Collections.Generic;

public class CircleSpawner : MonoBehaviour
{
    public float minSpawnRate = 2f;
    public float maxSpawnRate = 10f;
    public float minDiameter = 5f;
    public float maxDiameter = 10f;
    public int maxCircles = 20;
    public Color[] randomColors;
    
    private List<GameObject> circles = new List<GameObject>();
    private Camera mainCamera;
    private float spawnRate;
    private int circleCount = 0;  // Define the circleCount variable
    
    public void SetIntensity(float intensity)
    {
        spawnRate = Mathf.Lerp(maxSpawnRate, minSpawnRate, intensity / 100f);
        maxCircles = Mathf.FloorToInt(Mathf.Lerp(1, maxCircles, intensity / 100f));
        
        if (intensity == 0)
        {
            foreach (var circle in circles)
            {
                Destroy(circle);
            }
            circles.Clear();
        }
        else
        {
            CancelInvoke("SpawnCircle");
            InvokeRepeating("SpawnCircle", 0f, spawnRate);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (!IsTagDefined("Circle"))
        {
            Debug.Log("Tag 'Circle' does not exist. Creating it now.");
            UnityEditorInternal.InternalEditorUtility.AddTag("Circle");
        }

        bool overdosed = false;

        if (!overdosed) 
        {
            randomColors = new Color[]
            {
                // Vivid and intensified versions of primary colors
                new Color(1.0f, 0.0f, 0.0f, 0.8f),  // Vivid Red with 80% opacity
                new Color(0.0f, 0.0f, 1.0f, 0.8f),  // Vivid Blue with 80% opacity
                new Color(1.0f, 1.0f, 0.0f, 0.8f),  // Vivid Yellow with 80% opacity

                // Neon or fluorescent colors
                new Color(1.0f, 0.8f, 0.0f, 0.8f),  // Neon Orange with 80% opacity
                new Color(0.8f, 1.0f, 0.0f, 0.8f),  // Neon Green with 80% opacity
                new Color(1.0f, 0.0f, 1.0f, 0.8f),  // Neon Pink with 80% opacity

                // Earthy and natural tones
                new Color(0.5f, 0.3f, 0.1f, 0.8f),  // Earthy Brown with 80% opacity
                new Color(0.0f, 0.6f, 0.2f, 0.8f),  // Earthy Green with 80% opacity

                // Deep purples and pinks
                new Color(0.6f, 0.0f, 0.6f, 0.8f),  // Deep Purple with 80% opacity
                new Color(1.0f, 0.4f, 0.6f, 0.8f),  // Deep Pink with 80% opacity

                // Rainbow-like color patterns
                new Color(1.0f, 0.0f, 0.0f, 0.8f),  // Red with 80% opacity
                new Color(1.0f, 0.5f, 0.0f, 0.8f),  // Orange with 80% opacity
                new Color(1.0f, 1.0f, 0.0f, 0.8f),  // Yellow with 80% opacity
                new Color(0.0f, 1.0f, 0.0f, 0.8f),  // Green with 80% opacity
                new Color(0.0f, 0.5f, 1.0f, 0.8f),  // Blue with 80% opacity
                new Color(0.5f, 0.0f, 1.0f, 0.8f),  // Purple with 80% opacity

                // Shifting or morphing colors
                new Color(0.8f, 0.2f, 0.4f, 0.8f),  // Shifting Red-Purple with 80% opacity
                new Color(0.2f, 0.6f, 0.8f, 0.8f),  // Shifting Blue-Green with 80% opacity

                // Glowing or iridescent colors
                new Color(0.0f, 0.8f, 1.0f, 0.8f),  // Iridescent Turquoise with 80% opacity
                new Color(1.0f, 0.2f, 0.8f, 0.8f),  // Iridescent Pink with 80% opacity

                // Geometric patterns in various colors
                new Color(0.8f, 0.8f, 0.0f, 0.8f),  // Geometric Yellow with 80% opacity
                new Color(0.0f, 0.8f, 0.8f, 0.8f),  // Geometric Cyan with 80% opacity
                new Color(0.8f, 0.0f, 0.8f, 0.8f),  // Geometric Purple with 80% opacity

                // Warmer colors
                new Color(1.0f, 0.6f, 0.0f, 0.8f),  // Warm Orange with 80% opacity
                new Color(0.8f, 0.6f, 0.0f, 0.8f),  // Warm Gold with 80% opacity
                new Color(0.8f, 0.4f, 0.0f, 0.8f),  // Warm Brown with 80% opacity

                // Cool colors
                new Color(0.0f, 0.6f, 1.0f, 0.8f),  // Cool Blue with 80% opacity
                new Color(0.0f, 0.8f, 0.6f, 0.8f),  // Cool Teal with 80% opacity
                new Color(0.0f, 0.4f, 0.8f, 0.8f)   // Cool Deep Blue with 80% opacity
            };

        }
        else 
        {
            randomColors = new Color[]
            {
                // Vivid and intensified versions of primary colors
                new Color(1.0f, 0.0f, 0.0f, 1.0f),  // Vivid Red
                new Color(0.0f, 0.0f, 1.0f, 1.0f),  // Vivid Blue
                new Color(1.0f, 1.0f, 0.0f, 1.0f),  // Vivid Yellow

                // Neon or fluorescent colors
                new Color(1.0f, 0.8f, 0.0f, 1.0f),  // Neon Orange
                new Color(0.8f, 1.0f, 0.0f, 1.0f),  // Neon Green
                new Color(1.0f, 0.0f, 1.0f, 1.0f),  // Neon Pink

                // Earthy and natural tones
                new Color(0.5f, 0.3f, 0.1f, 1.0f),  // Earthy Brown
                new Color(0.0f, 0.6f, 0.2f, 1.0f),  // Earthy Green

                // Deep purples and pinks
                new Color(0.6f, 0.0f, 0.6f, 1.0f),  // Deep Purple
                new Color(1.0f, 0.4f, 0.6f, 1.0f),  // Deep Pink

                // Rainbow-like color patterns
                new Color(1.0f, 0.0f, 0.0f, 1.0f),  // Red
                new Color(1.0f, 0.5f, 0.0f, 1.0f),  // Orange
                new Color(1.0f, 1.0f, 0.0f, 1.0f),  // Yellow
                new Color(0.0f, 1.0f, 0.0f, 1.0f),  // Green
                new Color(0.0f, 0.5f, 1.0f, 1.0f),  // Blue
                new Color(0.5f, 0.0f, 1.0f, 1.0f),  // Purple

                // Shifting or morphing colors
                new Color(0.8f, 0.2f, 0.4f, 1.0f),  // Shifting Red-Purple
                new Color(0.2f, 0.6f, 0.8f, 1.0f),  // Shifting Blue-Green

                // Glowing or iridescent colors
                new Color(0.0f, 0.8f, 1.0f, 1.0f),  // Iridescent Turquoise
                new Color(1.0f, 0.2f, 0.8f, 1.0f),  // Iridescent Pink

                // Geometric patterns in various colors
                new Color(0.8f, 0.8f, 0.0f, 1.0f),  // Geometric Yellow
                new Color(0.0f, 0.8f, 0.8f, 1.0f),  // Geometric Cyan
                new Color(0.8f, 0.0f, 0.8f, 1.0f),  // Geometric Purple

                // Warmer colors
                new Color(1.0f, 0.6f, 0.0f, 1.0f),  // Warm Orange
                new Color(0.8f, 0.6f, 0.0f, 1.0f),  // Warm Gold
                new Color(0.8f, 0.4f, 0.0f, 1.0f),  // Warm Brown

                // Cool colors
                new Color(0.0f, 0.6f, 1.0f, 1.0f),  // Cool Blue
                new Color(0.0f, 0.8f, 0.6f, 1.0f),  // Cool Teal
                new Color(0.0f, 0.4f, 0.8f, 1.0f)   // Cool Deep Blue
            };
        }
    }

    private bool IsTagDefined(string tag)
    {
        foreach (string definedTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (definedTag == tag)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnCircle()
    {
        if (circleCount >= maxCircles)
        {
            // Stop spawning circles if the maximum limit is reached
            return;
        }

        Vector2 spawnPosition = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
        GameObject circle = new GameObject("Circle");

        float randomDiameter = Random.Range(minDiameter, maxDiameter);

        Rigidbody2D circleRigidbody = circle.AddComponent<Rigidbody2D>();
        circleRigidbody.gravityScale = 0;

        // Apply an initial force to make the circle move
        Vector2 initialForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        circleRigidbody.AddForce(initialForce.normalized * Random.Range(100f, 300f));

        SpriteRenderer circleRenderer = circle.AddComponent<SpriteRenderer>();
        circleRenderer.color = randomColors[Random.Range(0, randomColors.Length)];

        Texture2D circleTexture = CreateCircleTexture((int)(randomDiameter * 100), circleRenderer.color);
        circleRenderer.sprite = Sprite.Create(circleTexture, new Rect(0, 0, circleTexture.width, circleTexture.height), new Vector2(0.5f, 0.5f), 100);

        circleRenderer.sortingOrder = 0;

        circle.transform.position = spawnPosition;
        circle.transform.localScale = new Vector3(randomDiameter, randomDiameter, 1);

        // Add the circle to the list of circles and increment the count
        circles.Add(circle);
        circleCount++;
    }


    private Texture2D CreateCircleTexture(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size);
        int radius = size / 2;
        Vector2 center = new Vector2(radius, radius);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if ((center - new Vector2(x, y)).sqrMagnitude <= radius * radius)
                {
                    tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();

        return tex;
    }

    private void Update()
    {
        foreach (var circle in circles)
        {
            if (!IsCircleVisible(circle))
            {
                BounceCircleOffBoundary(circle);
            }
        }
    }

    private bool IsCircleVisible(GameObject circle)
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(circle.transform.position);
        return (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1);
    }

    private void BounceCircleOffBoundary(GameObject circle)
    {
        Rigidbody2D rb = circle.GetComponent<Rigidbody2D>();

        // Get the circle's position in viewport coordinates
        Vector3 viewPos = mainCamera.WorldToViewportPoint(circle.transform.position);

        // Determine the direction to bounce based on which boundary the circle is leaving
        Vector2 bounceDirection = Vector2.zero;

        if (viewPos.x < 0 || viewPos.x > 1)
        {
            bounceDirection.x = -Mathf.Sign(viewPos.x - 0.5f);
        }

        if (viewPos.y < 0 || viewPos.y > 1)
        {
            bounceDirection.y = -Mathf.Sign(viewPos.y - 0.5f);
        }

        // Apply the bounce force
        rb.AddForce(bounceDirection * 1f); // Adjust force as needed
    }
}
