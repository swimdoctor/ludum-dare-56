using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoScript : MonoBehaviour
{
    private Text nameText;
    private Text levelText;
    private Text statsText;
    private Text attackNameText;
    private Text attackDescText;
    private Text traitsText;


    private void Awake()
    {
        foreach (Text text in GetComponentsInChildren<Text>())
        {
            switch (text.gameObject.name)
            {
                case "Name Text":
                    nameText = text;
                    break;
                case "Level Text":
                    levelText = text;
                    break;
                case "Stats Text":
                    statsText = text;
                    break;
                case "Attack Name Text":
                    attackNameText = text;
                    break;
                case "Attack Description Text":
                    attackDescText = text;
                    break;
                case "Traits Text":
                    traitsText = text;
                    break;
                default:
                    break;

            }
        }
    }


    public void UpdateInfo(Creature c)
    {
        nameText.text = c.Name;
        levelText.text = c.getLevelString();
        statsText.text = c.getStatsString();
        attackNameText.text = c.primaryAttack.name;
        attackDescText.text = c.primaryAttack.getDescription();
        traitsText.text = c.getTraitsString();
    }
}
