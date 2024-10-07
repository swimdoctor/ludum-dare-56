using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BasicAttack
{
    public enum Attacks
    {
        Punch,
        LeafAttack,
        MagicAttack,
        FlameThrowerAttack,
        HealOrb,
        Slam,
    }

    public static List<BasicAttack> basicAttacksList = new List<BasicAttack>()
    {
        new Punch(),
        new LeafAttack(),
        new MagicAttack(),
        new FlameThrowerAttack(),
        new HealOrb(),
        new Slam(),
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



    protected string meleeOrRanged()
    {
        string meleeOrRanged = "";
        if (isMelee)
        {
            meleeOrRanged = "melee";
        }
        else if (isRanged)
        {
            meleeOrRanged = "ranged";
        }
        return meleeOrRanged;

    }
    public virtual string getDescription()
    {
        

        return $"Deals {NumString(minDamage)}-{NumString(maxDamage)} base {meleeOrRanged()} damage.";
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

        target.TakeKnockBack(knockBackAmount, attacker.transform.position, attacker);
        TriggerAttackTraits(attacker, target, damage, melee: true);

    }

    private void TriggerAttackTraits(UnitScript attacker, UnitScript target, float damage, bool melee = true)
    {
        foreach (Trait trait in attacker.stats.traitList)
        {
            trait.OnAttack(attacker, target, damage, melee:true);
        }
        if (melee)
        {
            foreach (Trait trait in target.stats.traitList)
            {
                trait.OnMeleeAttacked(target, attacker, damage);
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

    protected string NumString(float amount)
    {
        return amount.ToString("F2");
    }

}


public class Punch : BasicAttack
{
    public Punch()
    {
        name = "Punch";

        range = 1f;

        maxcooldown = 0.8f;

        minDamage = 17f;
        maxDamage = 22f;

        knockBackAmount = 3f;

        isMelee = true;
    }
    
}

public class Slam : BasicAttack
{
    public Slam()
    {
        name = "Slam";

        range = 1f;

        maxcooldown = 1.6f;

        minDamage = 34f;
        maxDamage = 44f;

        knockBackAmount = 5f;

        isMelee = true;
    }

}

public class LeafAttack : BasicAttack
{
    int num_projectiles = 3;
    public LeafAttack()
    {
        name = "Leaf Attack";

        range = 5f;

        maxcooldown = 2f;

        minDamage = 8f;
        maxDamage = 12f;

        knockBackAmount = 0.5f;

        isRanged = true;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        for (int i = 0; i < num_projectiles; i++)
        {
            attacker.FireProjectile(attacker, this, attacker.leafProjectilePrefab, GetDirection(attacker, target, inaccuracy:0.52f), target);
        }
    }
    public override string getDescription()
    {
        return $"Fires a volley of {num_projectiles} leaves, each dealing {NumString(minDamage)}-{NumString(maxDamage)} base {meleeOrRanged()} damage.";
    }
}

public class MagicAttack : BasicAttack
{
    public MagicAttack()
    {
        name = "Magic Attack";

        range = 7f;

        maxcooldown = 1f;

        minDamage = 14f;
        maxDamage = 18f;

        knockBackAmount = 1.5f;

        isRanged = true;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        attacker.FireProjectile(attacker, this, attacker.magicProjectilePrefab, GetDirection(attacker, target), target);
    }
    public override string getDescription()
    {
        return $"Fires a seeking projectile that deals {NumString(minDamage)}-{NumString(maxDamage)} base {meleeOrRanged()} damage.";
    }
}

public class FlameThrowerAttack : BasicAttack
{
    public FlameThrowerAttack()
    {
        name = "Flamethrower";

        range = 5f;

        maxcooldown = 0.06f;

        minDamage = 1.2f;
        maxDamage = 1.6f;

        knockBackAmount = 0.05f;

        isRanged = true;
    }
    public override void Activate(UnitScript attacker, UnitScript target)
    {
        Vector2 dir = GetDirection(attacker, target, inaccuracy:0.25f);
        attacker.FireProjectile(attacker, this, attacker.flameProjectilePrefab, dir, target);
    }
    public override string getDescription()
    {
        return $"Fires a constant stream of flames, each dealing {NumString(minDamage)}-{NumString(maxDamage)} base {meleeOrRanged()} damage.";
    }
}

public class HealOrb : BasicAttack
{
    public HealOrb()
    {
        name = "Healing Orb";

        range = 8f;

        maxcooldown = 1f;

        minDamage = -22f;
        maxDamage = -18f;

        knockBackAmount = 0f;

        isHeal = true;
    }

    public override void Activate(UnitScript attacker, UnitScript target)
    {
        attacker.FireProjectile(attacker, this, attacker.healProjectilePrefab, GetDirection(attacker, target), target, targetAlly:true);
    }
    public override string getDescription()
    {
        return $"Fires a healing orb that restores {NumString(-minDamage)}-{NumString(-maxDamage)} health to allies.";
    }
}