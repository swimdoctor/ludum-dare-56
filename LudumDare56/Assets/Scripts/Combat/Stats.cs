using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] public float basicAttackCooldown;
    [SerializeField] public float basicAttackPower;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float maxHealth;

    [SerializeField] public BasicAttack primaryAttack;

    [SerializeField] public float aggro;

    [SerializeField] int orderInParty;

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
