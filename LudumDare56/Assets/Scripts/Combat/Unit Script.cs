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

    public static int num_units;

    public int unit_id;


    private Rigidbody2D rb;


    private float primaryAttackCooldown;
    private float secondaryAttackCooldown;

    

    public bool team;

    public float currentHP;

    public Creature stats;

    private UnitScript currentTarget;

    private BasicAttack primaryAttack;

    // Projectile Prefabs
    public GameObject leafProjectilePrefab;
    public GameObject magicProjectilePrefab;
    public GameObject flameProjectilePrefab;
    public GameObject healProjectilePrefab;

    public void OnSpawned() 
    {
        unit_id = num_units;
        num_units++;

        rb = GetComponent<Rigidbody2D>();
        // stats = GetComponent<Creature>();

        stats.getBattleStats();
        foreach (Trait trait in stats.traitList)
        {
            trait.ModifyStats(this);
        }

        primaryAttack = stats.primaryAttack;
        primaryAttackCooldown = primaryAttack.maxcooldown;
        currentHP = stats.maxHealth;

        currentTarget = null;

    }

    public void OnCombatStart()
    {
        Draggable dragscript = GetComponent<Draggable>();
        if (dragscript != null)
        {
            Destroy(dragscript);
        }

        // Set the start position to current position
        stats.startPosition = transform.position;

        foreach (Trait trait in stats.traitList)
        {
            trait.OnBattleStart(this);
        }
    }

    private void OnDeath()
    {

        foreach (Trait trait in stats.traitList)
        {
            trait.OnDie(this);
        }

        gameObject.layer = 13;
        units.Remove(this);
        Destroy(gameObject);
    }

    private void Update() 
    {
        if (CombatManager.Instance.combatState == CombatManager.State.Before)
        {
            if (!team)
            {
                RestrictPosition();
            }
        } 
        else if (CombatManager.Instance.combatState == CombatManager.State.During)
        {
            primaryAttackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        if (CombatManager.Instance.combatState == CombatManager.State.During)
        {
            doDuringCombat();
        }
        
    }

    private void doDuringCombat()
    {
        if (currentTarget == null)
        {
            currentTarget = FindNewTarget();
        }

        if (currentTarget == null)
        {
            // FindNewTarget did not find a valid target

        }
        else
        {
            if (currentTarget.currentHP <= 0)
            {
                currentTarget = FindNewTarget();
            }

            float distToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            if (distToTarget > primaryAttack.range * stats.rangeModifier)
            {   // Move towards target
                Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
                rb.AddForce(direction * stats.moveSpeed * 5, ForceMode2D.Force);
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
                    primaryAttack.Activate(this, currentTarget);

                    float cooldownMod = 1f;
                    if (primaryAttack.isMelee)
                    {
                        cooldownMod = stats.meleeAttackSpeed;
                    } else if (primaryAttack.isRanged)
                    {
                        cooldownMod = stats.meleeAttackSpeed;
                    }
                    primaryAttackCooldown = primaryAttack.maxcooldown / cooldownMod;
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

    public UnitScript FindNewTarget()
    {
        UnitScript bestTarget = null;
        float highestAggro = 0;
        float closestTargetDist = 99999999;

        bool targetTeam;
        if (primaryAttack.isHeal)
        {
            targetTeam = team;
        } else
        {
            targetTeam = !team;
        }

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

    private void RestrictPosition()
    {
        float boundsY = 4;
        float boundX1 = -7;
        float boundX2 = -1;
        if (transform.position.x > boundX2)
        {
            transform.position = new Vector3(boundX2, transform.position.y, transform.position.z);
        }
        if (transform.position.x < boundX1)
        {
            transform.position = new Vector3(boundX1, transform.position.y, transform.position.z);
        }
        if (transform.position.y > boundsY)
        {
            transform.position = new Vector3(transform.position.x, boundsY, transform.position.z);
        }
        if (transform.position.y < -boundsY)
        {
            transform.position = new Vector3(transform.position.x, -boundsY, transform.position.z);
        }
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

        if (currentHP < 0) { // die

            OnDeath();

            return true;
        } else if (currentHP > stats.maxHealth)
        {
            currentHP = stats.maxHealth;
        }
        return false;
    }

    public void TakeKnockBack(float amount, Vector2 sourcePosition, UnitScript sourceUnit)
    {
        Vector2 pos = transform.position;
        Vector2 direction = (pos - sourcePosition).normalized;

        amount *= sourceUnit.stats.knockbackOutgoing;
        amount *= stats.knockbackIncoming;

        rb.AddForce(direction * amount, ForceMode2D.Impulse);
    }
    public void FireProjectile(UnitScript source, BasicAttack attack, GameObject type, Vector2 direction, UnitScript target, bool targetAlly = false)
    {
        GameObject projectile = Instantiate(type, transform.position, transform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.Initialize(source, direction, attack, targetAlly, target);
    }

}
