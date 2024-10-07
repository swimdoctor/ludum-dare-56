using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using static Trait;

public class Creature
{
    public enum BasicCreature
    {
        Burger,
        Steampunk,
        Plant,
        Knight,
        Candle,
        Slugduck,
        WaterDragon,
        SwirlyDragon,
        Cardboard,
        Panda,
    }

    // Let's not use this list anymore, refer to GetBasicCreature() and use the enums
	/*public static List<Creature> basicCreatureDict = new List<Creature>()//We need better names lmao
    {
		new Creature("Burger", new List<string>(){"Bur", "ger"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Steampunk", new List<string>(){"Steam", "punk"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Plant", new List<string>(){"Plant", "guy"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f),
		new Creature("Knight", new List<string>(){"Sir ", "knight"}, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f)
	};*/

	public bool singleLeg;
	public Sprite head, torso, leftLeg, rightLeg, leftArm, rightArm, headAccessory, backAccessory;
    public AnimatorController headAnim, torsoAnim;
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
    public float meleeAttackSpeed; // Divide all attack cooldowns by this value
    public float rangedAttackSpeed;
    public float moveSpeed;
    public float maxHealth;

    public float knockbackOutgoing;
    public float knockbackIncoming;

    public float rangeModifier;

    public int orderInParty;
    public int aggro;

    public int mergeLevel;

    

    private Creature(string spriteName, List<string> name, BasicAttack primaryAttack, float attackPowerStat, float attackSpeedStat, float healthStat, float moveSpeedStat, List<Trait> traits)
    {
        //Stats
        this.name = name;
        this.primaryAttack = primaryAttack;
        this.attackPowerStat = attackPowerStat;
        this.attackSpeedStat = attackSpeedStat;
        this.healthStat = healthStat;
        this.moveSpeedStat = moveSpeedStat;

        knockbackOutgoing = 1f;
        knockbackIncoming = 1f;

        rangeModifier = 1f;

        // Traits
        traitList = new List<Trait>(traits);

        //Sprites
        head = Resources.Load<Sprite>(spriteName + "_Head");
        torso = Resources.Load<Sprite>(spriteName + "_Torso") ;
        leftLeg = (Resources.Load<Sprite>(spriteName + "_LeftLeg") ) ?? (Resources.Load<Sprite>(spriteName + "_Leg") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		rightLeg = (Resources.Load<Sprite>(spriteName + "_RightLeg") ) ?? (Resources.Load<Sprite>(spriteName + "_Leg") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		leftArm = (Resources.Load<Sprite>(spriteName + "_LeftArm") ) ?? (Resources.Load<Sprite>(spriteName + "_Arm") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		rightArm = (Resources.Load<Sprite>(spriteName + "_RightArm") ) ?? (Resources.Load<Sprite>(spriteName + "_Arm") ) ?? (Resources.Load<Sprite>(spriteName + "_Limb") );
		headAccessory = Resources.Load<Sprite>(spriteName + "_HeadAccessory") ;
        backAccessory = Resources.Load<Sprite>(spriteName + "_BackAccessory") ;

        //Animations
        headAnim = Resources.Load<AnimatorController>(spriteName + "_HeadAnim");
        torsoAnim = Resources.Load<AnimatorController>(spriteName + "_TorsoAnim");

    }

    private float getLevelModifier()
    {
        return (mergeLevel * 0.5f) + 1f;
    }

    public void getBattleStats()
    {
        float attackPower = (attackPowerStat * 1f + 0.5f) * getLevelModifier();
        meleeAttackPower = attackPower;
        rangedAttackPower = attackPower;

        float attackSpeed = (attackSpeedStat * 1f + 0.5f) * getLevelModifier();
        meleeAttackSpeed = attackSpeed;
        rangedAttackSpeed = attackSpeed;


        moveSpeed = (moveSpeedStat * 1.5f + 0.5f);
        maxHealth = (healthStat * 100f + 75f) * getLevelModifier();
    }

    public static List<Trait> getTraitsFromPool(List<Traits> pool, int num = 2) { 
        // Returns a list containing num traits chosen from pool
        List<Trait> list = new List<Trait>();
        List<Traits> traitPool = new List<Traits>(pool); // Create shallow copy of list so we can remove
        for (int i = 0; i < num; i++)
        {
            int index = UnityEngine.Random.Range(0, traitPool.Count);

            Trait trait = traitsList[(int)traitPool[index]];

            list.Add(trait);
            traitPool.RemoveAt(index);
        }
        return list;
    }

    public static Creature GetBasicCreature(BasicCreature i)
    {
        switch (i)
        {
            case BasicCreature.Burger:
                return new Creature("Burger",
                    new List<string>() { "Bur", "ger" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .7f, .2f, .9f, .25f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.Healthy,
                        Traits.Pushy,
                        Traits.SlowTwitchMuscle }
                    )
                );

            case BasicCreature.Steampunk:
                return new Creature("Steampunk", new List<string>() { "Steam", "punk" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .35f, .9f, .3f, .75f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.GlassCannon,
                        Traits.Ranger,
                        Traits.meleeFireProjectile,
                        Traits.Haste }
                    )
                );
            case BasicCreature.Plant:
                return new Creature("Plant", new List<string>() { "Plant", "age" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.LeafAttack], .4f, .5f, .75f, .75f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Noticable,
                        Traits.Healthy,
                        Traits.Lifesteal,
                        Traits.Brawler,
                        Traits.Thorns,
                    }
                    )
                );
            case BasicCreature.Knight:
                return new Creature("Knight", new List<string>() { "Sir ", "knight" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .8f, .3f, .7f, .15f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.meleeFireProjectile,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Strength,
                        Traits.Pushy}
                    )
                );
            case BasicCreature.Candle:
                return new Creature("Knight", new List<string>() { "Candle", "flame" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.FlameThrowerAttack], .9f, .9f, .1f, .05f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.GlassCannon,
                        Traits.Haste,
                        Traits.Ranger,
                        Traits.Strength,}
                    )
                );
            case BasicCreature.Slugduck:
                return new Creature("Knight", new List<string>() { "Slug", "duck" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .6f, .4f, .5f, .6f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Healthy,
                        Traits.Noticable,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Strength,}
                    )
                );
            case BasicCreature.SwirlyDragon:
                return new Creature("Knight", new List<string>() { "Swirly", "dragon" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .35f, .8f, .3f, .1f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.meleeFireProjectile,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Vampiric,}
                    )
                );
            case BasicCreature.Cardboard:
                return new Creature("Knight", new List<string>() { "Card", "board" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .5f, .5f, .5f, .5f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Pushy,
                        Traits.meleeFireProjectile,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Strength,
                        Traits.Ranger,
                        Traits.Vampiric,}
                    )
                );
            case BasicCreature.Panda:
                return new Creature("Knight", new List<string>() { "Trash", "panda" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .8f, .4f, .5f, .3f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.meleeFireProjectile,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Strength,}
                    )
                );


            default:
                return null;

        }
    }


    public static List<Creature> GenerateTeam(int difficulty, int maxSize = 5)
    {
        // Generates a team of enemy creatures.
        // int difficulty determines the total combined merge level of all creatures
 

        List<Creature> newTeam = new List<Creature>();

        for (int i = 0; i < difficulty; i++)
        {
            if (i >= maxSize) 
            {
                // Merge a random creature on the team with a basic creature
                Creature basicCreature = GetBasicCreature(GetRandomEnumValue<BasicCreature>());

                int index = UnityEngine.Random.Range(0, newTeam.Count);
                Creature creatureToMerge = newTeam[index];

                Creature newCreature = Merge(creatureToMerge, basicCreature);

                newTeam.RemoveAt(index); // Remove creature that got merged with
                newTeam.Add(newCreature);
                
            } 
            else
            {
                // Create a new creature and add it to the team
                Creature creature = GetBasicCreature(GetRandomEnumValue<BasicCreature>());
                newTeam.Add(creature);
            }
            
        }

        return newTeam;
    }
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }


    public static Creature Merge(Creature A, Creature B)
    {
        Creature AB = new Creature("Knight", new List<string>() { "Merged ", "Creature" }, BasicAttack.basicAttacksList[0], .7f, .2f, .9f, .25f,
                    new List<Trait>());

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
        List<Trait> takefromlist = new List<Trait>();

        foreach (Trait t in A.traitList)
        {
            if (!takefromlist.Contains(t))
            {
                takefromlist.Add(t);
            }
        }

        for (int i = 0; i < maxTraitCount; i++)
        {
            if (takefromlist.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, takefromlist.Count);
                Trait trait = takefromlist[index];

                takefromlist.RemoveAt(index);
                if (!traits.Contains(trait))
                {
                    traits.Add(trait);
                }
            }
        }

        AB.traitList = traits;
        


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
