using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    [SerializeField] public BasicAttack primaryAttack = BasicAttack.basicAttacksList[1];

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

    // Start is called before the first frame update
    void Start()
    {
        //Set position of unit to slot corresponding to number in party
    }

    // Update is called once per frame
    void Update()
    {
        //Check for aggro
    }
}
