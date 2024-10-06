using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public GameObject combatantPrefab;

    // Static (global) reference to the single existing instance of the object
    private static CombatManager _instance = null;

    // Public property to allow access to the Singleton instance
    // A property is a member that provides a flexible mechanism to read, write, or compute the value of a data field.
    public static CombatManager Instance
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

        List<Creature> team1 = GetTestTeam();
        List<Creature> team2 = GetTestTeam();

        LoadTeams(team1, team2);

    }
    public void LoadTeams(List<Creature> playerTeam, List<Creature> enemyTeam)
    {
        foreach (Creature creature in playerTeam)
        {
            // We need to create a Sprite, and put the creature in that sprite
            GameObject newGuy = Instantiate(combatantPrefab);
            UnitScript unit = newGuy.GetComponent<UnitScript>();

            unit.stats = creature;
            unit.team = false;
            unit.transform.position = new Vector2(Random.Range(-6, 0), Random.Range(-8, 8));
            unit.OnSpawned();
        }

        foreach (Creature creature in enemyTeam)
        {
            // We need to create a Sprite, and put the creature in that sprite
            GameObject newGuy = Instantiate(combatantPrefab);
            UnitScript unit = newGuy.GetComponent<UnitScript>();

            unit.stats = creature;
            unit.team = true;
            unit.transform.position = new Vector2(Random.Range(0, 8), Random.Range(-8, 8));
            unit.OnSpawned();
        }
    }

    private List<Creature> GetTestTeam()
    {
        return new List<Creature>()
        {
            new Creature(),
            new Creature(),
            new Creature(),
        };
    }
}