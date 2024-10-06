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
        new MagicAttack(),
        new FlameThrowerAttack(),
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

    protected Vector2 GetDirection(UnitScript attacker, UnitScript target, float inaccuracy = 0)
    {
        // helper method for projectile firing

        Vector2 dir = (target.transform.position - attacker.transform.position).normalized;
        float radians = Mathf.Atan2(dir.y, dir.x);
        radians += Random.Range(-inaccuracy, inaccuracy);
        dir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        return dir;
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
        attacker.FireProjectile(attacker, this, attacker.leafProjectilePrefab, GetDirection(attacker, target), target);
    }
}

public class MagicAttack : BasicAttack
{
    public MagicAttack()
    {
        name = "Magic Attack";

        range = 7f;

        maxcooldown = 3f;

        minDamage = 2f;
        maxDamage = 3f;

        knockBackAmount = 2f;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        attacker.FireProjectile(attacker, this, attacker.magicProjectilePrefab, GetDirection(attacker, target), target);
    }
}

public class FlameThrowerAttack : BasicAttack
{
    public FlameThrowerAttack()
    {
        name = "Flamethrower";

        range = 5f;

        maxcooldown = 0.06f;

        minDamage = 0.05f;
        maxDamage = 0.1f;

        knockBackAmount = 0f;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        Vector2 dir = GetDirection(attacker, target, inaccuracy:0.25f);
        attacker.FireProjectile(attacker, this, attacker.flameProjectilePrefab, dir, target);
    }
}