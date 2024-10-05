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

    protected float calcDamage(UnitScript sourceUnit, float minDamage, float maxDamage, bool melee = false, bool ranged = false)
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
        float damage = calcDamage(attacker, minDamage, maxDamage, melee: true);
        target.changeHP(-damage, attacker);

        target.takeKnockBack(knockBackAmount, attacker.transform.position);
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
