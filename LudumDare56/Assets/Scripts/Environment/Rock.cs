using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Rock : MonoBehaviour, IInteractable
{
    [SerializeField] private int difficulty;  
    public string CombatScene;

    public void Interact(GameObject User)
    {
        SceneManager.LoadScene(CombatScene);
        Debug.Log("Interacted with wizard tower");
    }


}
