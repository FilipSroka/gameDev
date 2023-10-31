using System.Collections;
using UnityEngine;

public class LeakPipe : MonoBehaviour
{
    public GameObject capsulePrefab;
    public float spawnInterval = 3f;
    public float capsuleSpeed = 5f;

    private void Start()
    {
        StartCoroutine(SpawnCapsules());
    }

    private IEnumerator SpawnCapsules()
    {
        while (true)
        {
            // Instantiate a new capsule
            GameObject newCapsule = Instantiate(capsulePrefab, transform.position, Quaternion.identity);

            // Add a Rigidbody2D component to the capsule
            Rigidbody2D rb2d = newCapsule.GetComponent<Rigidbody2D>();

            // Apply gravity to the capsule
            rb2d.gravityScale = 1.0f;

            // Apply a downward force to make it fall
            rb2d.AddForce(Vector2.down * capsuleSpeed, ForceMode2D.Impulse);

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
