using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public static List<UnitScript> units;


    private Rigidbody2D rb;


    private float primaryAttackCooldown;
    private float secondaryAttackCooldown;

    [SerializeField] bool team;

    private float currentHP;

    public Stats stats;

    private UnitScript currentTarget;

    private void OnEnable() 
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        currentHP = stats.maxHealth;

        if (units == null)
        {
            Debug.Log("Creating new list");
            units = new List<UnitScript>();
        }

        if (!units.Contains(this)) // Put this unit in the list of units
        {
            units.Add(this);
            
            Debug.Log("Adding unit to list");
        }
        currentTarget = null;

    }

    private void OnDeath()
    {
        units.Remove(this);
        Destroy(gameObject);
    }

    private void Update() 
    {
        primaryAttackCooldown -= Time.deltaTime;
    }
    private void FixedUpdate() {
        if (currentTarget == null)
        {
            currentTarget = FindNewTarget();
            Debug.Log("Target is Null, Targeting" + currentTarget);
        }

        if (currentTarget == null)
        {
            // FindNewTarget did not find a valid target
            
        } else
        {
            if (currentTarget.currentHP <= 0)
            {
                currentTarget = FindNewTarget();
                Debug.Log("Target is Dead, Targeting" + currentTarget);
            }

            float distToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            if (distToTarget > stats.primaryAttack.range)
            {   // Move towards target
                Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
                rb.AddForce(direction * stats.moveSpeed, ForceMode2D.Force);
            }
            else
            {
                if (primaryAttackCooldown < 0)
                {
                    // Attack
                    Debug.Log(this + " Attacking " + currentTarget);
                    stats.primaryAttack.Attack(this, currentTarget);
                    primaryAttackCooldown = stats.primaryAttack.maxcooldown;
                }
            }
        }

        
    }

    private UnitScript FindNewTarget()
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

        return bestTarget;
        

    }

    public bool ChangeHP(float amount)
    {
        currentHP += amount;
        if (currentHP < 0) { // die

            OnDeath();

            return true;
        } else if (currentHP > stats.maxHealth)
        {
            currentHP = stats.maxHealth;
        }
        return false;
    }
}
