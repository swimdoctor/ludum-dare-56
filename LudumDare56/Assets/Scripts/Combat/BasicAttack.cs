using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BasicAttack
{
    public string name;
    public string description;
    public float maxcooldown;
    public delegate void myDelegate(UnitScript attacker, UnitScript target);
    public int attackFunction;
    public float range;
    public bool isHeal;

    

    public BasicAttack(string name, string description, float range, float cooldown, int attackFunction, bool isHeal = false)
    {
        this.name = name;
        this.description = description;
        this.range = range;
        this.maxcooldown = cooldown;
        this.attackFunction = attackFunction;
        this.isHeal = isHeal;

    }

    public void Attack(UnitScript attacker, UnitScript target)
    {
        switch (this.attackFunction)
        {
            case 0: Punch(attacker, target); break;
            case 1: break;
        }
    }

    public static void Punch(UnitScript attacker, UnitScript target)
    {
        float minDamage = 1;
        float maxDamage = 2;

        float damage = Random.Range(minDamage, maxDamage + 1);

        target.ChangeHP(-damage);
        
        Debug.Log("hi");
    }
}
