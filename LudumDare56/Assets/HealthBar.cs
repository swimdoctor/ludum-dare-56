using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private UnitScript unit;
    public HealthBar healthBar;
    private void onEnable()
    {
        unit = GetComponent<UnitScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float health = 
        
    }

    void TakeDamage()
    {
        healthBar -= 
           
    }
}
