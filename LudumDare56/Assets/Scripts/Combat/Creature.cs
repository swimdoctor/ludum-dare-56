using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

    private Creature() { }

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
    }

    public string Name
    {
        get
        {
            string str = "";
            foreach (string s in name)
            {
                str += s;
            }
            return str;
        }
    }

    public string getStatsString()
    {
        return 
            $"{(attackPowerStat * 100f).ToString("0")} ({getAttackPower().ToString("0.00")}x)\n"+
            $"{(attackSpeedStat * 100f).ToString("0")} ({getAttackSpeed().ToString("0.00")}x)\n"+
            $"{(healthStat * 100f).ToString("0")} ({getMaxHealth().ToString("0")} HP)\n"+
            $"{(moveSpeedStat * 100f).ToString("0")}\n"
            ;
    }

    public string getTraitsString()
    {
        string value = "";
        foreach (Trait t in traitList)
        {
            value += t.name + ": " + t.GetDescription() + "\n";
        }
        return value;
    }

    public string getLevelString()
    {
        string value;
        switch (mergeLevel)
        {
            case 0:
                value = "Tier 1: Plain";
                break;
            case 1:
                value = "Tier 2: Weird";
                break;
            case 2:
                value = "Tier 3: Freaky";
                break;
            case 3:
                value = "Tier 4: Grotesque";
                break;
            default:
                value = "Tier 0";
                break;
        }
        return value;
    }

    public float getLevelModifier()
    {
        return (mergeLevel * 0.5f) + 1f;
    }

    public void getBattleStats()
    {
        float attackPower = getAttackPower();
        meleeAttackPower = attackPower;
        rangedAttackPower = attackPower;

        float attackSpeed = getAttackSpeed();
        meleeAttackSpeed = attackSpeed;
        rangedAttackSpeed = attackSpeed;


        moveSpeed = getMoveSpeed();
        maxHealth = getMaxHealth();
    }

    private float getAttackPower()
    {
        return (attackPowerStat * 1f + 0.5f) * getLevelModifier();
    }
    private float getAttackSpeed()
    {
        return (attackSpeedStat * 1f + 0.5f) * getLevelModifier();
    }
    private float getMoveSpeed()
    {
        return (moveSpeedStat * 1.5f + 0.5f);
    }
    private float getMaxHealth()
    {
        return (healthStat * 100f + 75f) * getLevelModifier();
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
                return new Creature("Burger", new List<string>() { "Bur", "ger" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Punch], .7f, .2f, .9f, .25f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.Healthy,
                        Traits.Pushy,
                        Traits.SlowTwitchMuscle }
                    )
                );

            case BasicCreature.Steampunk:
                return new Creature("Steampunk", new List<string>() { "Steam", "punk" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.MagicAttack], .35f, .9f, .3f, .75f,
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
                return new Creature("Knight", new List<string>() { "Sir ", "knight" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Slam], .8f, .3f, .7f, .15f,
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
                return new Creature("Knight", new List<string>() { "Slug", "duck" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.HealOrb], .6f, .4f, .5f, .6f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Healthy,
                        Traits.Noticable,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Strength,}
                    )
                );
            case BasicCreature.WaterDragon:
                return new Creature("Knight", new List<string>() { "Water", "dragon" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.HealOrb], .5f, .5f, .65f, .7f,
                    getTraitsFromPool(new List<Traits>()
                    {
                        Traits.Juggernaut,
                        Traits.meleeFireProjectile,
                        Traits.Agile,
                        Traits.Brawler,
                        Traits.Vampiric,}
                    )
                );
            case BasicCreature.SwirlyDragon:
                return new Creature("Knight", new List<string>() { "Swirly", "dragon" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.MagicAttack], .35f, .8f, .3f, .1f,
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
                return new Creature("Knight", new List<string>() { "Trash", "panda" }, BasicAttack.basicAttacksList[(int)BasicAttack.Attacks.Slam], .8f, .4f, .5f, .3f,
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

                Debug.Log(creatureToMerge+ " " + basicCreature);
                Debug.Log($"Merge {creatureToMerge.mergeLevel} {basicCreature.mergeLevel}, i={index}");
                Creature newCreature = Merge(creatureToMerge, basicCreature);


                newTeam.Add(newCreature);
                newTeam.RemoveAt(index); // Remove creature that got merged with



            } 
            else
            {
                Debug.Log("Making new creature, count = " + newTeam.Count);
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
        Creature AB = new Creature();

        AB.name = new List<string>();
        AB.name.AddRange(A.name);
        AB.name.AddRange(B.name);

        AB.mergeLevel = Mathf.Max(A.mergeLevel, B.mergeLevel) + 1;
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
            takefromlist.Add(t);
        }
        foreach (Trait t in B.traitList)
        {
            takefromlist.Add(t);
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

		AB.head = UnityEngine.Random.value < .5f ? A.head : B.head;
		AB.torso = UnityEngine.Random.value < .5f ? A.torso : B.torso;
		AB.leftLeg = UnityEngine.Random.value < .5f ? A.leftLeg : B.leftLeg;
		AB.rightLeg = UnityEngine.Random.value < .5f ? A.rightLeg : B.rightLeg;
		AB.leftArm = UnityEngine.Random.value < .5f ? A.leftArm : B.leftArm;
        AB.rightArm = UnityEngine.Random.value < .5f ? A.rightArm : B.rightArm;
		AB.headAccessory = UnityEngine.Random.value < .5f ? A.headAccessory : B.headAccessory;
		AB.backAccessory = UnityEngine.Random.value < .5f ? A.backAccessory : B.backAccessory;

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
