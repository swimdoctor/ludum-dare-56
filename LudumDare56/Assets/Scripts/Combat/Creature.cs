using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
	public static List<Creature> creatureDict = new List<Creature>()//We need better names lmao
    {
		new Creature("Burger", new List<string>(){"Bur", "ger"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Steampunk", new List<string>(){"Steam", "punk"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Plant", new List<string>(){"Plant", "guy"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Knight", new List<string>(){"Sir ", "knight"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f)
	};

	public bool singleLeg;
	public Sprite head, torso, leftLeg, rightLeg, leftArm, rightArm, headAccessory, backAccessory;
	public List<string> name;


    [SerializeField] public BasicAttack primaryAttack = BasicAttack.basicAttacksList[Random.Range(0, 5)];

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

        throw new NotImplementedException("Merge not implemented");


        return AB;
    }
}
