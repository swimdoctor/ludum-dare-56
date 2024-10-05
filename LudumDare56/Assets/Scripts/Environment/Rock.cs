using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, IInteractable
{
    

    public void Interact(GameObject User)
    {
        Debug.Log("Interacted with rock");
    }


}
