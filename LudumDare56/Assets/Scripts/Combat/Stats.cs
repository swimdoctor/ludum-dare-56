using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public List<string> name;
    public string Name
    {
        get
        {
            string str = "";
            foreach(string s in name)
                str += s;
            return str;
        }
    }

    [SerializeField] public BasicAttack primaryAttack = BasicAttack.basicAttacksList[0];

    // Traits that determine stats in battle, these do not change at any point during a battle
    [Range(0.0f, 1.0f)]
    public float attackPowerStat = 0.5f;
    [Range(0.0f, 1.0f)]
    public float attackSpeedStat = 0.5f;
    [Range(0.0f, 1.0f)]
    public float healthStat = 0.5f;
    [Range(0.0f, 1.0f)]
    public float moveSpeedStat = 0.5f;
    

    // Traits
    public List<Trait> traitList = new List<Trait>();

    // Stats that are used in battle
    [SerializeField] public float meleeAttackPower; // Multiply all attacks by this value
    [SerializeField] public float rangedAttackPower;
    [SerializeField] public float meleeAttackSpeed; // Multiply all attack cooldowns by this value
    [SerializeField] public float rangedAttackSpeed;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxHealth;
    [SerializeField] public int orderInParty;
    [SerializeField] public int aggro;


    public void getBattleStats()
    {
        float attackPower = (attackPowerStat * 1f + 0.5f);
        meleeAttackPower = attackPower;
        rangedAttackPower = attackPower;

        float attackSpeed = (attackSpeedStat * -1f + 1.5f);
        meleeAttackSpeed = attackSpeed;
        rangedAttackSpeed = attackSpeed;

        moveSpeed = (moveSpeedStat * 1f + 0.5f);
        maxHealth = (healthStat * 20f + 5f);



    }

    public static Stats Merge(Stats statA, Stats statB)
    {
        Stats stats = new Stats();

        //Name
        stats.name = MergeName(statA.name, statB.name);

        return stats;
	}

    public static List<string> MergeName(List<string> a, List<string> b, int rec = 10)
    {
		List<string> name = new List<string>();

		List<string> longer, shorter;
		if(a.Count > a.Count)
		{
			longer = new List<string>(a);
			shorter = new List<string>(b);
		}
		else
		{
			longer = new List<string>(b);
			shorter = new List<string>(a);
		}

		name = longer;
		for(int i = 0; i < (shorter.Count+1)/2; i++)
		{
			name.Insert(2*i+1, shorter[i]);
		}
		for(int i = 0; i < (shorter.Count)/2; i++)
		{
			name.Insert(name.Count - 1 - 2*i, shorter[shorter.Count-1-i]);
		}

		for(int i = 0; i < name.Count; i++)
		{
			if(Random.value < .5f && name.Count > 0)
			{
				name.RemoveAt(i);
			}
		}

		string strA = "";
		foreach(string s in a)
			strA += s;
		string strB = "";
		foreach(string s in b)
			strB += s;

		string str = "";
		foreach(string s in name)
			str += s;

		if(rec > 0 && (str == strA || str == strB))
			name = MergeName(a, b, rec-1);

		return name;
	}

}
