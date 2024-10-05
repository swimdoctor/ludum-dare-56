using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BasicAttack
{

    public static List<BasicAttack> basicAttacksList = new List<BasicAttack>()
    {
        new Punch(),
        new LeafAttack(),
    };
    public string name;
    public string description;
    public float maxcooldown;
    public float range;
    public bool isHeal;

    public bool isMelee;
    public bool isRanged;

    public float minDamage;
    public float maxDamage;

    public float knockBackAmount = 1f;

    


    public virtual string getDescription()
    {
        string meleeOrRanged = "";
        if (isMelee)
        {
            meleeOrRanged = "melee";
        } else if (isRanged)
        {
            meleeOrRanged = "ranged";
        }

        return $"Deals {minDamage}-{maxDamage} base {meleeOrRanged} damage";
    }

    public float calcDamage(UnitScript sourceUnit, bool melee = false, bool ranged = false)
    {
        float damage = Random.Range(minDamage, maxDamage);

        if (melee)
        {
            damage *= sourceUnit.stats.meleeAttackPower;
        }
        if (ranged)
        {
            damage *= sourceUnit.stats.rangedAttackPower;
        }

        return damage;
    }


    public virtual void Activate(UnitScript attacker, UnitScript target)
    {
        float damage = calcDamage(attacker, melee: true);
        target.ChangeHP(-damage, attacker);

        target.TakeKnockBack(knockBackAmount, attacker.transform.position);
        TriggerAttackTraits(attacker, target, melee: true);

    }

    private void TriggerAttackTraits(UnitScript attacker, UnitScript target, bool melee = true)
    {
        foreach (Trait trait in attacker.stats.traitList)
        {
            trait.OnAttack(attacker, target);
        }
        if (melee)
        {
            foreach (Trait trait in target.stats.traitList)
            {
                trait.OnMeleeAttacked(target, attacker);
            }
        }

    }

    protected Vector2 GetDirection(UnitScript attacker, UnitScript target)
    {
        // helper method for projectile firing
        return (target.transform.position - attacker.transform.position).normalized;
    }

    

}


public class Punch : BasicAttack
{
    public Punch()
    {
        name = "Punch";

        range = 1f;

        maxcooldown = 2f;

        minDamage = 1.5f;
        maxDamage = 2f;

        knockBackAmount = 1f;
    }
    
}

public class LeafAttack : BasicAttack
{
    public LeafAttack()
    {
        name = "Leaf Attack";

        range = 5f;

        maxcooldown = 2f;

        minDamage = 1.5f;
        maxDamage = 2f;

        knockBackAmount = 1f;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        attacker.FireProjectile(attacker, this, attacker.leafProjectilePrefab, GetDirection(attacker, target));
    }
}
