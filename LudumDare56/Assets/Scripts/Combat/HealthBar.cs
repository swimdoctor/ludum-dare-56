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
    private SpriteRenderer sr;

    void Start()
    {
        unit = transform.parent.GetComponent<UnitScript>();
        maxHealth = unit.stats.maxHealth;
        currentHealth = maxHealth;
        healthBarTransform = GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        if (unit.team)
            sr.color = new Color(1f, 0f, 1f);
        else
            sr.color = new Color(1f, 0f, 0f);
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