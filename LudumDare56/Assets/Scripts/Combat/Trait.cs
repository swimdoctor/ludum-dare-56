using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
 
public class Trait
{
    public enum Traits
    {
        Healthy,
        Agile,
        Lifesteal,
        meleeFireProjectile,
        GlassCannon,
        FastTwitchMuscle,
        SlowTwitchMuscle,
        Noticable,
        Ranger,
        Brawler,
        Pushy,
        Juggernaut,
    }

    public static List<Trait> traitsList = new List<Trait>()
    {
        new Healthy(),
        new Agile(),
        new LifeSteal(),
        new meleeFireProjectile(),
        new GlassCannon(),
        new FastTwitchMuscle(),
        new SlowTwitchMuscle(),
        new Noticable(),
        new Ranger(),
        new Brawler(),
        new Pushy(),
        new Juggernaut(),
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

    public virtual void OnAttack(UnitScript unit, UnitScript target, bool melee)
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

    protected string NumString(float amount)
    {
        return amount.ToString("F2");
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
        return ($"Increases max health by {NumString(modifier)}x.");
    }
    
    public override void ModifyStats(UnitScript unit)
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
   
    private float modifier = 0.2f;

    public override string GetDescription()
    {
        return ($"On melee attack, heal for {NumString(modifier*100)}% of the enemy's current health");
    }

    public override void OnAttack(UnitScript unit, UnitScript target, bool melee)
    {
        if (melee)
        {
            unit.ChangeHP(modifier * target.currentHP, unit);
        }
        
    }
}
class meleeFireProjectile : Trait
{
    public meleeFireProjectile()
    {
        name = "Energy Slash";
    }
    private float percentChance = 0.15f;
    
    public override string GetDescription()
    {
        return ($"Has a {NumString(percentChance * 100)}% chance to fire a projectile on melee hit ");
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
    public override void OnAttack(UnitScript unit, UnitScript target, bool melee)
    {
        float randomChance = Random.value;
       
        if(randomChance > percentChance)
        {
            unit.FireProjectile(unit, new MagicAttack(), unit.magicProjectilePrefab, GetDirection(unit, target), target);
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
    private float positiveModifier = 1.5f;
    private float negativeModifier = 0.5f;
    public override string GetDescription()
    {
        return ($"increases attack power and attack speed by {NumString(positiveModifier)}x, but reduces max health by {NumString(negativeModifier)}x");
    }
    public new void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth *= negativeModifier;
        unit.stats.attackSpeedStat *= positiveModifier;
        unit.stats.meleeAttackPower *= positiveModifier;
        unit.stats.rangedAttackPower *= positiveModifier;
    }
}
class FastTwitchMuscle : Trait
{
    public FastTwitchMuscle()
    {
        name = "Fast Twitch Muscle Fibers";
    }
    private float healthBuff = 1.25f;
    private float attackBuff = 1.5f;

    public override string GetDescription()
    {
        return ($"Boosts health by {NumString(healthBuff)}x and attack by {NumString(attackBuff)}x at start of battle, but attack power decays over time");
    }
    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth *= healthBuff;
        unit.stats.meleeAttackPower *= attackBuff;
        unit.stats.rangedAttackPower *= attackBuff;
    }
    public override void OnBattleStart(UnitScript unit)
    {
        CombatManager.StartCoroutineUsingManager(Reduce(unit));
        
    }
    private IEnumerator Reduce(UnitScript unit)
    {
        while (true)
        {
            unit.stats.meleeAttackPower *= 0.925f;
            unit.stats.rangedAttackPower *= 0.925f;
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
    private float healthDebuff = 0.75f;
    private float attackDebuff = 0.6f;

    public override string GetDescription()
    {
        return ($"Debuffs health to {NumString(healthDebuff)}x and attack to {NumString(attackDebuff)}x at start of battle, but attack power grows over time.");
    }
    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.maxHealth *= healthDebuff;
        unit.stats.meleeAttackPower *= attackDebuff;
        unit.stats.rangedAttackPower *= attackDebuff;
    }
    public override void OnBattleStart(UnitScript unit)
    {
        CombatManager.StartCoroutineUsingManager(Increase(unit));

    }
    private IEnumerator Increase(UnitScript unit)
    {
        while (true)
        {
            unit.stats.meleeAttackPower += 0.05f;
            unit.stats.rangedAttackPower += 0.05f;
            yield return new WaitForSeconds(1f);
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
class Agile : Trait
{
    public Agile()
    {
        name = "Agile";
    }

    private float modifier = 1.5f;

    public override string GetDescription()
    {
        return ($"Increases move speed by {NumString(modifier)}x.");
    }

    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.moveSpeed *= modifier;
    }
}
class Ranger : Trait
{
    public Ranger()
    {
        name = "Ranger";
    }

    private float modifier = 1.25f;

    public override string GetDescription()
    {
        return ($"Increases ranged damage and attack speed by {NumString(modifier)}x.");
    }

    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.rangedAttackPower *= modifier;
        unit.stats.rangedAttackSpeed *= modifier;
    }
}
class Brawler : Trait
{
    public Brawler()
    {
        name = "Brawler";
    }

    private float modifier = 1.25f;

    public override string GetDescription()
    {
        return ($"Increases melee damage and attack speed by {NumString(modifier)}x.");
    }

    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.meleeAttackPower *= modifier;
        unit.stats.meleeAttackSpeed *= modifier;
    }
}
class Pushy : Trait
{
    public Pushy()
    {
        name = "Pushy";
    }

    private float modifier = 2f;

    public override string GetDescription()
    {
        return ($"Increases knockback dealt by {NumString(modifier)}x.");
    }

    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.knockbackOutgoing *= modifier;
    }
}
class Juggernaut : Trait
{
    public Juggernaut()
    {
        name = "Juggernaut";
    }

    public override string GetDescription()
    {
        return ($"Takes no knockback.");
    }

    public override void ModifyStats(UnitScript unit)
    {
        unit.stats.knockbackIncoming = 0;
    }
}


// TODO: fix this
/*class ExplodeOnDeath : Trait
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
}*/
