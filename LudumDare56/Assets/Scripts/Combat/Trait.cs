using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

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
        // Called when a unit is destroyed
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

class LifeSteal : Trait
{
    public LifeSteal()
    {
        name = "Lifesteal";
    }
   
    private float modifier = 0.25f;

    public override string GetDescription()
    {
        return ($"Steals {modifier}% of the enemy's health");
    }

    public new void ModifyStats(UnitScript unit)
    {
        unit.ChangeHP(modifier, unit);
    }
}
class meleeFireProjectile : Trait
{
    public meleeFireProjectile()
    {
        name = "Energy Slash";
    }
    private float percentChance = 0.10f;
    private float randomChance = Random.value;
    MagicAttack projectile;
    public override string GetDescription()
    {
        return ($"has a {percentChance}% chance to fire a projectile on melee hit ");
    }
    private Vector2 GetDirection(UnitScript attacker, UnitScript target, float inaccuracy = 0)
    {
        // helper method for projectile firing

        Vector2 dir = (target.transform.position - attacker.transform.position).normalized;
        float radians = Mathf.Atan2(dir.y, dir.x);
        radians += Random.Range(-inaccuracy, inaccuracy);
        dir = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        return dir;
    }
    public new void OnAttack(UnitScript unit, UnitScript target)
    {
        
       
        if(randomChance > percentChance)
        {
            unit.FireProjectile(unit,projectile,unit.magicProjectilePrefab, GetDirection(unit,target), target);
            //fire projectile
        }
        else
        {
            //do nothing
        }
    }

}
class GlassCannon : Trait
{
    public GlassCannon()
    {
           name = "Glass Cannon";
    }
    private float modifier = 1.5f;
    public override string GetDescription()
    {
        return ($"increases attack power and attack speed by {modifier}x, but reduces max health by {modifier}");
    }
    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth /= modifier;
        unit.stats.attackSpeedStat *= modifier;
        unit.stats.meleeAttackPower *= modifier;
        unit.stats.rangedAttackPower *= modifier;
    }
}
class FastTwitchMuscle : Trait
{
    public FastTwitchMuscle()
    {
        name = "Fast Twitch Muscle Fibers";
    }
    private float healthBuff = 10f;
    private float attackBuff = 20f;

    public override string GetDescription()
    {
        return ($"Massively boosts health by {healthBuff} and attack by {attackBuff} at start of battle, but reduces health and attack power throughout the battle");
    }
    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth += healthBuff;
        unit.stats.meleeAttackPower += attackBuff;
        unit.stats.rangedAttackPower += attackBuff;
    }
    public new void OnBattleStart(UnitScript unit)
    {
        Reduce(unit);
        
    }
    private IEnumerator Reduce(UnitScript unit)
    {
        while (true)
        {
            unit.stats.meleeAttackPower -= 0.1f;
            unit.stats.rangedAttackPower -= 0.1f;
            yield return new WaitForSeconds(1f);
        }
    }
}

class SlowTwitchMuscle : Trait
{
    public SlowTwitchMuscle()
    {
        name = "Slow Twitch Muscle Fibers";
    }
    private float healthDebuff = 5f;
    private float attackDebuff = 5f;

    public override string GetDescription()
    {
        return ($"Massively debuffs health by {healthDebuff} and attack by {attackDebuff} at start of battle, but increases health and attack power throughout the battle");
    }
    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth += healthDebuff;
        unit.stats.meleeAttackPower += attackDebuff;
        unit.stats.rangedAttackPower += attackDebuff;
    }
    public new void OnBattleStart(UnitScript unit)
    {
        Increase(unit);

    }
    private IEnumerator Increase(UnitScript unit)
    {
        while (true)
        {
            unit.stats.meleeAttackPower += 0.3f;
            unit.stats.rangedAttackPower += 0.3f;
            yield return new WaitForSeconds(4f);
        }
    }

}

class Noticable : Trait
{

    public Noticable() {
        name = "Noticable";
    }

    private int modifier = 2;

    public override string GetDescription() {
        return ($"The aggro range of this creature is increased by {modifier}");
    }

    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.aggro += modifier;
    }
}

class ExplodeOnDeath : Trait
{
    public ExplodeOnDeath() {
        name = "Kamikaze";
    }

    public override string GetDescription()
    {
        return ($"Blows up on death harming both friendly and enemy alike");
    }
    private float radius = 5f;
    private float damage = 20f;
    public new void OnDie(UnitScript unit)
    {
        RaycastHit2D hit = Physics2D.CircleCast(unit.transform.position,radius,Vector2.zero);
        UnitScript hitbox = hit.collider.GetComponent<UnitScript>();
        hitbox.ChangeHP(-damage, unit);

    }
}
