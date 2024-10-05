using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    private static List<UnitScript> units;


    private Rigidbody2D rb;


    [SerializeField] private float primaryAttackCooldown;
    [SerializeField] private float secondaryAttackCooldown;


    [SerializeField] private float attackRange = 1f;
    

    [SerializeField] bool team;

    private float currentHP;

    private Stats stats;

    private UnitScript currentTarget;

    private BasicAttack primaryAttack;

    private void OnEnable() 
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        primaryAttack = BasicAttacks.attacksList[0];
        primaryAttackCooldown = primaryAttack.maxcooldown;
        currentHP = stats.maxHealth;
        units.Add(this);
        if (true | !(units.Contains(this))) // Put this unit in the list of units
        {
            currentTarget = null;
        }

    }

    private void Update() 
    {
        primaryAttackCooldown -= Time.deltaTime;
    }
    private void FixedUpdate() {
        if (currentTarget == null)
        {
            FindNewTarget();
        }


        if (currentTarget.currentHP <= 0)
        {
            FindNewTarget();
        }

        float distToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        if (distToTarget > attackRange)
        {   // Move towards target
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
            rb.AddForce(direction*stats.moveSpeed);
        }
        else
        {
            if (primaryAttackCooldown < 0)
            {
                // Attack
                primaryAttack.Attack(this, currentTarget);
                primaryAttackCooldown = primaryAttack.maxcooldown;
            }
        }


        
    }

    private void FindNewTarget()
    {
        UnitScript bestTarget = null;
        float highestAggro = 0;
        float closestTargetDist = 99999999;

        bool targetTeam = !team; // Change this if healing
        Debug.Log(units);
        foreach (UnitScript unit in units) {
            if (unit != this && unit.team == targetTeam)
            {
                if (unit.stats.aggro > highestAggro)
                {
                    bestTarget = unit;

                }
                else if (unit.stats.aggro == highestAggro)
                {
                    float dist = Vector2.Distance(transform.position, unit.transform.position);
                    if (dist < closestTargetDist)
                    {
                        bestTarget = unit;
                        closestTargetDist = dist;
                    }
                }
            }
        }

        currentTarget = bestTarget;



    }

    public bool ChangeHP(float amount)
    {
        currentHP += amount;
        if (currentHP < 0) { // die
            return true;
        } else if (currentHP > stats.maxHealth)
        {
            currentHP = stats.maxHealth;
        }
        return false;
    }
}
