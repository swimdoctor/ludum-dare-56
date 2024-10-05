using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    [SerializeField] public BasicAttack primaryAttack = BasicAttacks.attacksList[0];
    [SerializeField] public float moveSpeed = 1;
    [SerializeField] public float maxHealth = 10;

    [SerializeField] public int orderInParty;

    [SerializeField] public int aggro = 1;

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
