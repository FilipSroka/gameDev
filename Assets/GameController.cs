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
    private float end;
    private Child last;

    private void Start()
    {
        // Create a list to keep track of the children.
        childrenList = new List<Child>();
        queue = new Queue<Child>();


        // Iterate through all children of the mapGameObject and add them to the list.
        foreach (Transform child in mapGameObject.transform)
            childrenList.Add(new Child(child));

        childrenList[0].changeState(0);
        queue.Enqueue(childrenList[0]);
        last = childrenList[0];

        // Randomly select two children from the list.
        int index1 = Random.Range(1, childrenList.Count);
        int index2;
        do
        {
            index2 = Random.Range(1, childrenList.Count);
        } while (index2 == index1);


        // index1 = 1; // DELETE

        end = getEnd(childrenList[index1]);
        childrenList[index1].changeState(end);
        queue.Enqueue(childrenList[index1]);
        last = childrenList[index1];

        // index2 = 2; // DELETE

        end = getEnd(childrenList[index2]);
        childrenList[index2].changeState(end);
        queue.Enqueue(childrenList[index2]);
        last = childrenList[index2];
    }

    private void Update() 
    {
        float playerPosition = player.transform.position.x;
        Transform tmp = last.transform.Find("Water");
        end = tmp.position.x + (tmp.localScale.x / 2);

        if (end - playerPosition < 30.0f)
        {
            List<Child> selection = getSelection();

            // Randomly select one child from the list.
            int index = Random.Range(0, selection.Count);
            end = getEnd(selection[index]);
            selection[index].changeState(end);
            queue.Enqueue(selection[index]);
            last = selection[index];

            if (queue.Count > 5)
                queue.Dequeue().changeState(end);
        }
    }

    private List<Child> getSelection() 
    {
        List<Child> selection = new List<Child>();
        int maxCount = childrenList.Max(child => child.count);
        foreach (Child c in childrenList)
            if (c.isAvailable) 
                for(int i = 0; i < maxCount-c.count+1; i++)
                    selection.Add(c);
        return selection;
    }

    private float getEnd(Child c)
    {
        Transform firstTransform = last.transform;
        Transform secondTransform = c.transform;

        // Assuming both objects are centered and their scale is based on their width
        float firstWidth = firstTransform.Find("Water").localScale.x;
        float secondWidth = secondTransform.Find("Water").localScale.x;

        // Calculate the x-coordinate for the right edge of the first block
        float firstEndX = firstTransform.position.x + firstWidth * 0.5f;

        Debug.Log(firstEndX+"END");

        // Calculate the desired x-coordinate for the left edge of the second block to touch the right edge of the first block
        float xCoordinate = firstEndX + secondWidth * 0.5f;

        Debug.Log(xCoordinate+"X");

        return xCoordinate;
    }
}

public class Child
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
            count++;
        }
        else 
        {
            transform.position = new Vector2(end, 25f);
            isAvailable = true;
        }
    }

}
