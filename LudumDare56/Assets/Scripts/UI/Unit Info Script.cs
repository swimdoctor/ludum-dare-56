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

    // Static (global) reference to the single existing instance of the object
    private static UnitInfoScript _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static UnitInfoScript Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {

        // If an instance of the GameManager does not already exist (meaning this is the first time we've created this class)
        if (_instance == null)
        {
            // Make this object the one that _instance points to
            _instance = this;
        }
        // Otherwise if an instance already exists and it's not this one
        else
        {
            Destroy(gameObject);
        }

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
