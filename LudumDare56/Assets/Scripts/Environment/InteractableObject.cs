using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    //Function called to Trigger an interaction with this object
    public bool Interact(GameObject user)
    {
        Debug.Log("Interacted with object");
        return false;
    }

}
