using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Properties;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{

    public GameObject textbox;
    public GameObject eInteract;
    public GameObject CanvasObject;

    void Start()
    {
        DisableeInteract();
    }
    /* private void OnTriggerEnter2D(Collider2D collision)
     {
         Debug.Log("hit");
         ActiveeInteract();
     }

     private void OnTriggerExit2D(Collider2D collision)
     {
         Debug.Log("exit");
         DisableeInteract();
     }
    */

    public void DisableeInteract()
    {
        eInteract.SetActive(false);
        textbox.SetActive(false);
    }

    public void ActiveeInteract()
    {
        eInteract.SetActive(true);
    }
    public void interactWithNPC()
    {
        textbox.SetActive(true);
    }
}
