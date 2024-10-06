using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class CharacterDictionary
{
    public static List<CharacterDictionary> characters = new List<CharacterDictionary>()
    {

    };


    public string name = "placeholder name";

    public string GetName()
    {
        return name;
    }

    public virtual string GetDescription()
    {
        return "Placeholder Description";
    }


    class Burger : Creature
    {
        public Burger()
        {
            attackPowerStat = 0.7f;
            attackSpeedStat = 0.2f;
            healthStat = 0.9f;
            moveSpeedStat = 0.25f;
        }

    }
}
