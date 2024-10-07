using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class WizardTower : MonoBehaviour, IInteractable
{


    public string CombatScene;


    public void Interact(GameObject User)
    {
        SceneManager.LoadScene(CombatScene);
        Debug.Log("Interacted with wizard tower");
    }


}
