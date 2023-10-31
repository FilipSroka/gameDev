using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrugCollector : MonoBehaviour
{
    private int drugs = 0;
    

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
      if (collision.gameObject.CompareTag("Drug"))
      {
        Destroy(collision.gameObject);
        drugs++;
        Debug.Log("Drugs: " + drugs);
       
      }
        
    }
}
