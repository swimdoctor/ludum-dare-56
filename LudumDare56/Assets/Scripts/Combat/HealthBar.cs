using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    UnitScript unit;
    public float maxHealth;
    private float currentHealth;
    private Transform healthBarTransform;

    void Start()
    {
        unit = transform.parent.GetComponent<UnitScript>();
        maxHealth = unit.stats.maxHealth;
        currentHealth = maxHealth;
        healthBarTransform = GetComponent<Transform>();
    }

    void Update(){

        currentHealth = unit.currentHP;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        transform.localScale = new Vector3(healthPercentage, .1f, 1);
    }
}