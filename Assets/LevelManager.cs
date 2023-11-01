using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<Transform> segments;
    public GameObject player;

    private Queue<Transform> availableSegments;
    private Transform currentSegment;
    private Transform nextSegment;
    private float currentSegmentWidth;

    private void Start()
    {
        if (segments.Count == 0) return;

        // Initialize the availableSegments queue with a shuffled list of all segments except the first one
        availableSegments = new Queue<Transform>(Shuffle(segments.GetRange(1, segments.Count - 1)));

        // The first segment in the list is always the starting segment
        currentSegment = segments[0];
        UpdateCurrentSegmentWidth();
    }

    private void Update()
    {
        // Check if player has moved past halfway of the current segment
        if (player.transform.position.x > currentSegment.position.x + currentSegmentWidth / 2 && nextSegment == null)
        {
            LoadNextSegment();
        }

        // Check if the player has finished the current segment
        if (nextSegment != null && player.transform.position.x > currentSegment.position.x + currentSegmentWidth)
        {
            MoveToNextSegment();
        }
    }

    private void MoveToNextSegment()
    {
        // Add currentSegment back to the queue
        availableSegments.Enqueue(currentSegment);

        // Update the current segment to the next segment
        currentSegment = nextSegment;
        UpdateCurrentSegmentWidth();
        
        nextSegment = null;  // Reset nextSegment
    }

    private void LoadNextSegment()
    {
        if (availableSegments.Count == 0) return;  // Safety check

        // Get a random segment from the queue
        nextSegment = availableSegments.Dequeue();
        
        // Calculate the width of the next segment
        float nextSegmentWidth = nextSegment.Find("Water").GetComponent<BoxCollider2D>().bounds.size.x;

        // Position it right after the current segment by using the local offset from its parent 
        float nextSegmentLocalOffset = nextSegment.Find("Water").GetComponent<BoxCollider2D>().bounds.extents.x;
        float currentSegmentLocalOffset = currentSegment.Find("Water").GetComponent<BoxCollider2D>().bounds.extents.x;

        // Calculate the position for the next segment
        nextSegment.position = new Vector2(currentSegment.position.x + currentSegmentLocalOffset + nextSegmentLocalOffset, currentSegment.position.y);
    }

    private void UpdateCurrentSegmentWidth()
    {
        currentSegmentWidth = currentSegment.Find("Water").GetComponent<BoxCollider2D>().size.x;
    }

    private List<Transform> Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Transform temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}
