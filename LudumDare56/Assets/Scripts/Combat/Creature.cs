using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
	public static List<Creature> basicCreatureDict = new List<Creature>()//We need better names lmao
    {
		new Creature("Burger", new List<string>(){"Bur", "ger"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Steampunk", new List<string>(){"Steam", "punk"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Plant", new List<string>(){"Plant", "guy"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Knight", new List<string>(){"Sir ", "knight"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f)
	};

	public bool singleLeg;
	public Sprite head, torso, leftLeg, rightLeg, leftArm, rightArm, headAccessory, backAccessory;
	public List<string> name;


    [SerializeField] public BasicAttack primaryAttack = BasicAttack.basicAttacksList[UnityEngine.Random.Range(0, 5)];

    public Vector2 startPosition = Vector2.zero;

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
    public float meleeAttackPower; // Multiply all attacks by this value
    public float rangedAttackPower;
    public float meleeAttackSpeed; // Multiply all attack cooldowns by this value
    public float rangedAttackSpeed;
    public float moveSpeed;
    public float maxHealth;
    public int orderInParty;
    public int aggro;

    // 
    public int mergeLevel;

    public Creature() { }

    private Creature(string spriteName, List<string> name, BasicAttack primaryAttack, float attackPowerStat, float attackSpeedStat, float healthStat, float moveSpeedStat)
    {
        //Stats
        this.name = name;
        this.primaryAttack = primaryAttack;
        this.attackPowerStat = attackPowerStat;
        this.attackSpeedStat = attackSpeedStat;
        this.healthStat = healthStat;
        this.moveSpeedStat = moveSpeedStat;

        //Sprites
        head = Resources.Load<Sprite>(spriteName + "_Head");
        torso = Resources.Load<Sprite>(spriteName + "_Torso") ;
        leftLeg = (Resources.Load<Sprite>(spriteName + "_LeftLeg") ) ?? (Resources.Load<Sprite>(spriteName + "_Leg") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		rightLeg = (Resources.Load<Sprite>(spriteName + "_RightLeg") ) ?? (Resources.Load<Sprite>(spriteName + "_Leg") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		leftArm = (Resources.Load<Sprite>(spriteName + "_LeftArm") ) ?? (Resources.Load<Sprite>(spriteName + "_Arm") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		rightArm = (Resources.Load<Sprite>(spriteName + "_RightArm") ) ?? (Resources.Load<Sprite>(spriteName + "_Arm") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		headAccessory = Resources.Load<Sprite>(spriteName + "_HeadAccessory") ;
        backAccessory = Resources.Load<Sprite>(spriteName + "_BackAccessory") ;
    }

    public void getBattleStats()
    {
        float attackPower = (attackPowerStat * 1f + 0.5f);
        meleeAttackPower = attackPower;
        rangedAttackPower = attackPower;

        float attackSpeed = (attackSpeedStat * -1f + 1.5f);
        meleeAttackSpeed = attackSpeed;
        rangedAttackSpeed = attackSpeed;

        moveSpeed = (moveSpeedStat * 1f + 0.5f);
        maxHealth = (healthStat * 100f + 75f);

    }

    public static Creature Merge(Creature A, Creature B)
    {
        Creature AB = new Creature();

        AB.mergeLevel = Math.Max(A.mergeLevel, B.mergeLevel) + 1;
        if (AB.mergeLevel > 3)
        {
            AB.mergeLevel = 3;
        }

        AB.attackPowerStat = MergeStat(A.attackPowerStat, B.attackPowerStat);
        AB.attackSpeedStat = MergeStat(A.attackSpeedStat, B.attackSpeedStat);
        AB.moveSpeedStat = MergeStat(A.moveSpeedStat, B.moveSpeedStat);
        AB.healthStat = MergeStat(A.healthStat, B.healthStat);

        if (UnityEngine.Random.Range(0, 2) == 1) 
        {
            AB.primaryAttack = A.primaryAttack;
        } 
        else
        {
            AB.primaryAttack = B.primaryAttack;
        }

        int maxTraitCount = AB.mergeLevel + 2;
        List<Trait> traits = new List<Trait>();
        List<Trait> takefromlist;
        int index;

        // Copy these lists to remove from later
        List<Trait> parent1traits = new List<Trait>(A.traitList);
        List<Trait> parent2traits = new List<Trait>(B.traitList);

        for (int i = 0; i <= maxTraitCount; i++)
        {
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                takefromlist = parent1traits;
            }
            else
            {
                takefromlist = parent2traits;
            }

            index = UnityEngine.Random.Range(0, takefromlist.Count);
            Trait trait = takefromlist[index];
            takefromlist.RemoveAt(index);
        }
        


        return AB;
    }

    private static float MergeStat(float parent1stat, float parent2stat)
    {
        float variancePositive = 0.15f;
        float varianceNegative = -0.07f;

        float par1percent = UnityEngine.Random.Range(0f, 1f);
        float par2percent = 1f - par1percent;

        float passedDownStat = (par1percent * parent1stat + par2percent * parent2stat);

        passedDownStat += UnityEngine.Random.Range(varianceNegative, variancePositive);

        if (passedDownStat < 0f)
        {
            passedDownStat = 0f;
        }
        else if (passedDownStat > 1f)
        {
            passedDownStat = 1f;
        }

        return passedDownStat;
    }
}
