using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    // Reference to the Map GameObject that contains the children.
    public List<Child> childrenList;
    public Queue<Child> queue;
    public GameObject mapGameObject;
    public GameObject player;
    public float end;
    public float width;
    private List<int> manager;

    private void Start()
    {
        // Create a list to keep track of the children.
        childrenList = new List<Child>();
        queue = new Queue<Child>();


        // Iterate through all children of the mapGameObject and add them to the list.
        foreach (Transform child in mapGameObject.transform)
        {
            childrenList.Add(new Child(child));
        }

        // Randomly select two children from the list.
        int index1 = Random.Range(0, childrenList.Count);
        int index2;
        do
        {
            index2 = Random.Range(0, childrenList.Count);
        } while (index2 == index1);

        // Move the selected children to the specified 2D coordinates and set their availability to false.
        childrenList[index1].changeState(end);
        queue.Enqueue(childrenList[index1]);

        end += width;
        childrenList[index2].changeState(end);
        queue.Enqueue(childrenList[index2]);
    }

    private void Update() 
    {
        float playerPosition = player.transform.position.x;

        if (end - playerPosition < 30.0f)
        {
            List<Child> selection = getSelection();

            // Randomly select one child from the list.
            int index = Random.Range(0, selection.Count);

            end += width;
            selection[index].changeState(end);
            queue.Enqueue(childrenList[index]);

            if (queue.Count > 5)
            {
                queue.Dequeue().changeState(end);
            }
        }
    }

    private List<Child> getSelection() 
    {
        List<Child> selection = new List<Child>();
        int minVal = childrenList.Min(child => child.count);

        foreach (Child c in childrenList)
        {
            if (c.isAvailable && c.count == minVal) 
            {
                selection.Add(c);
            }
        }
        return selection;
    }
}

public class Child : MonoBehaviour
{
    public Transform transform;
    public bool isAvailable;
    public int count;

    public Child(Transform t)
    {
        transform = t;
        isAvailable = true;
        count = 0;
    }

    public void changeState(float end) 
    {
        if (isAvailable) {
            transform.position = new Vector2(end, -2f);
            isAvailable = false;
            count += 1;
        }
        else 
        {
            transform.position = new Vector2(0f, -10f);
            isAvailable = true;
        }
    }

}
