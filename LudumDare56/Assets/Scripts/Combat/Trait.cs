using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trait
{
    public static List<Trait> traitsList = new List<Trait>()
    {
        new Healthy(),
    };


    public string name = "placeholder name";
    
    public string getName()
    {
        return name;
    }

    public virtual string GetDescription()
    {
        return "Placeholder Description";
    }

    public virtual void ModifyStats(UnitScript unit)
    {
        // Called after battle stat values have been retrieved from base stats (Immediately after GetBattleStats)
    }

    public virtual void OnBattleStart(UnitScript unit)
    {
        // Called when the battle begins
    }

    public virtual void OnAttack(UnitScript unit, UnitScript target)
    {
        // Called after the unit attacks
    }

    public virtual void OnMeleeAttacked(UnitScript unit, UnitScript attacker)
    {
        // Called after the unit is targeted by a melee attack
    }

    public virtual void OnTakeDamage(UnitScript unit)
    {
        // Called upon taking any type of damage
    }

    public virtual void OnDie(UnitScript unit)
    {

    }

}

class Healthy : Trait
{
    public Healthy()
    {
        name = "Healthy";
    }

    private float modifier = 1.25f;

    public override string GetDescription()
    { 
        return ($"Increases max health by {modifier}x.");
    }
    
    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth *= modifier;
    }
}