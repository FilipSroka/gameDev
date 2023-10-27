using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Reference to the Map GameObject that contains the children.
    public List<(Transform, bool)> childrenList;
    public Queue<(Transform, int)> queue;
    public GameObject mapGameObject;
    public GameObject player;
    public float end;
    public float width;

    private void Start()
    {
        // Create a list to keep track of the children.
        childrenList = new List<(Transform, bool)>();
        queue = new Queue<(Transform, int)>();

        // Iterate through all children of the mapGameObject and add them to the list.
        foreach (Transform child in mapGameObject.transform)
        {
            childrenList.Add((child, true));
        }

        // Randomly select two children from the list.
        int index1 = Random.Range(0, childrenList.Count);
        int index2;
        do
        {
            index2 = Random.Range(0, childrenList.Count);
        } while (index2 == index1);

        // Move the selected children to the specified 2D coordinates and set their availability to false.
        childrenList[index1].Item1.position = new Vector2(end, -2f);
        childrenList[index1] = (childrenList[index1].Item1, false);
        queue.Enqueue((childrenList[index1].Item1, index1));

        end += width;
        childrenList[index2].Item1.position = new Vector2(end, -2f);
        childrenList[index2] = (childrenList[index2].Item1, false);
        queue.Enqueue((childrenList[index2].Item1, index2));
    }

    private void Update() 
    {
        float playerPosition = player.transform.position.x;

        if (end - playerPosition < 30.0f)
        {
            // Randomly select one child from the list.
            int index;
            do
            {
                index = Random.Range(0, childrenList.Count);
            } while (!childrenList[index].Item2);

            end += width;
            childrenList[index].Item1.position = new Vector2(end, -2f);
            childrenList[index] = (childrenList[index].Item1, false);
            queue.Enqueue((childrenList[index].Item1, index));

            if (queue.Count > 5)
            {
                (Transform t, int i) tmp = queue.Dequeue();
                tmp.t.position = new Vector2(0f, -10f);
                childrenList[tmp.i] = (tmp.t, true);
            }
        }
    }
}

