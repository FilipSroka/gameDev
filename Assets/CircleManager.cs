using UnityEngine;

public class CircleManager : MonoBehaviour
{
    public CircleSpawner circleSpawner;

    // This function will be called by PlayerMovement
    public void SpawnCirclesWithIntensity(float intensity)
    {
        circleSpawner.SetIntensity(intensity);
    }
}
