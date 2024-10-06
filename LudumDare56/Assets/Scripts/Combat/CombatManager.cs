using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public GameObject combatantPrefab;

    public Button startButton;

    public enum State
    {
        Before,
        During,
        After
    }

    public State combatState;

    public List<UnitScript> listUnits;

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

        SetupCombat(team1, team2);

    }

    private void FixedUpdate()
    {
        int allyCount = 0;
        int enemyCount = 0;
        foreach (UnitScript unit in listUnits)
        {
            if (unit.team)
            {
                allyCount++;
            }
            else
            {
                enemyCount++;
            }
        }

        if (allyCount == 0)
        {
            EndGame();
            // TODO: Lose

        }
        else if (enemyCount == 0)
        {
            EndGame();
            // TODO: Win
        }

    }

    private void EndGame()
    {
        if (combatState == State.During)
        {
            combatState = State.After;
        }
    }

    public void SetupCombat(List<Creature> playerTeam, List<Creature> enemyTeam)
    {
        combatState = State.Before;

        foreach (Creature creature in playerTeam)
        {
            // We need to create a Sprite, and put the creature in that sprite
            GameObject newGuy = Instantiate(combatantPrefab);
            UnitScript unit = newGuy.GetComponent<UnitScript>();

            newGuy.AddComponent<Draggable>();

            unit.stats = creature;
            unit.team = false;

            if (unit.stats.startPosition == Vector2.zero)
            {
                unit.transform.position = new Vector2(Random.Range(-8, 0), Random.Range(-5, 5));
            } else
            {
                unit.transform.position = unit.stats.startPosition;
            }

            unit.gameObject.layer = 10;

            unit.OnSpawned();

            listUnits.Add(unit);
        }

        foreach (Creature creature in enemyTeam)
        {
            // We need to create a Sprite, and put the creature in that sprite
            GameObject newGuy = Instantiate(combatantPrefab);
            UnitScript unit = newGuy.GetComponent<UnitScript>();

            unit.stats = creature;
            unit.team = true;
            unit.transform.position = new Vector2(Random.Range(2, 8), Random.Range(-5, 5));

            unit.gameObject.layer = 11;

            unit.OnSpawned();

            listUnits.Add(unit);
        }

        UnitScript.units = listUnits;

        startButton.gameObject.SetActive(true);
    }

    public void StartCombat()
    {
        if (combatState == State.Before)
        {
            foreach (UnitScript unit in listUnits)
            {
                unit.OnCombatStart();
            }
            combatState = State.During;

            startButton.gameObject.SetActive(false);
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