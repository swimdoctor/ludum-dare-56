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

    [SerializeField] public BasicAttack primaryAttack = BasicAttacks.attacksList[0];
    [SerializeField] public float moveSpeed = 1;
    [SerializeField] public float maxHealth = 10;
    [SerializeField] public int orderInParty;
    [SerializeField] public int aggro = 1;

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
