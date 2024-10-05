using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class UnitScript : MonoBehaviour
{
    public static List<UnitScript> units;


    private Rigidbody2D rb;


    private float primaryAttackCooldown;
    private float secondaryAttackCooldown;

    

    public bool team;

    public float currentHP;

    public Stats stats;

    private UnitScript currentTarget;

    private BasicAttack primaryAttack;

    // Projectile Prefabs
    public GameObject leafProjectilePrefab;

    private void OnEnable() 
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();

        stats.getBattleStats();
        foreach (Trait trait in stats.traitList)
        {
            trait.ModifyStats(this);
        }

        primaryAttack = stats.primaryAttack;
        primaryAttackCooldown = primaryAttack.maxcooldown;
        Debug.Log($"Attack range: {primaryAttack.range}");
        currentHP = stats.maxHealth;

        if (units == null)
        {
            units = new List<UnitScript>();
        }

        if (!units.Contains(this)) // Put this unit in the list of units
        {
            units.Add(this);
        }
        currentTarget = null;

    }

    private void OnDeath()
    {

        foreach (Trait trait in stats.traitList)
        {
            trait.OnDie(this);
        }

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
            Debug.Log("Target is Null, Targeting " + currentTarget);
        }

        if (currentTarget == null)
        {
            // FindNewTarget did not find a valid target
            
        } else
        {
            if (currentTarget.currentHP <= 0)
            {
                currentTarget = FindNewTarget();
                Debug.Log("Target is Dead, Targeting " + currentTarget);
            }

            float distToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            if (distToTarget > primaryAttack.range)
            {   // Move towards target
                Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
                rb.AddForce(direction * stats.moveSpeed*5, ForceMode2D.Force);
            }
            else
            {
                // Stop movement
                if (rb.velocity.magnitude > 0)
                { 
                    rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.75f * Time.fixedDeltaTime);
                }


                if (primaryAttackCooldown < 0)
                {
                    // Attack
                    Debug.Log(this + " Attacking " + currentTarget);
                    primaryAttack.Activate(this, currentTarget);
                    primaryAttackCooldown = primaryAttack.maxcooldown;
                }
            }
        }

        limitVelocity();
    }

    private void limitVelocity()
    {
        if (rb.velocity.magnitude > stats.moveSpeed)
        {
            // Calculate the excess velocity
            Vector2 excessVelocity = rb.velocity - rb.velocity.normalized * stats.moveSpeed;

            // Apply a counterforce in the opposite direction of the excess velocity
            rb.AddForce(-excessVelocity.normalized * 0.5f, ForceMode2D.Impulse);
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

    public bool ChangeHP(float amount, UnitScript source=null)
    {
        if (amount < 0)
        { // Trigger damage taken traits
            foreach (Trait trait in stats.traitList)
            {
                trait.OnTakeDamage(this);
            }
        }

        currentHP += amount;

        Debug.Log($"Changed HP by {amount}, current {currentHP}");
        if (currentHP < 0) { // die

            OnDeath();

            return true;
        } else if (currentHP > stats.maxHealth)
        {
            currentHP = stats.maxHealth;
        }
        return false;
    }

    public void TakeKnockBack(float amount, Vector2 source)
    {
        Vector2 pos = transform.position;
        Vector2 direction = (pos - source).normalized;
        rb.AddForce(direction * amount, ForceMode2D.Impulse);
    }
    public void FireProjectile(UnitScript source, BasicAttack attack, GameObject type, Vector2 direction, bool targetAlly = false)
    {
        GameObject projectile = GameObject.Instantiate(type, transform.position, transform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.source = source;
        projectileScript.direction = direction;
        projectileScript.attack = attack;

        projectileScript.isHealProjectile = targetAlly;

        projectileScript.OnSpawn();


    }

}
