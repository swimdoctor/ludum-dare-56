using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public string OverworldScene;
    public GameObject combatantPrefab;

    public Button startButton;

    public GameObject unitInfoPanel;
    private UnitInfoScript unitInfo;
    private FadeInOut unitInfoFade;

    public GameObject victoryUI;
    [SerializeField] private Text victoryText;
    private FadeInOut victoryFade;

    public static Creature reward;
    public static bool giveReward;

    public enum State
    {
        Before,
        During,
        After
    }

    public State combatState;

    public List<UnitScript> listUnits;
    private List<UnitScript> listAllUnits; // Includes dead units

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

        unitInfo = unitInfoPanel.GetComponent<UnitInfoScript>();
        unitInfoFade = unitInfo.GetComponent<FadeInOut>();
        
        victoryFade = victoryUI.GetComponent<FadeInOut>();

        List<Creature> team1 = CreatureManager.instance.party;
        List<Creature> team2 = Creature.GenerateTeam(7, maxSize: 3);

        SetupCombat(team1, team2);

        giveReward = false;
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
            if (combatState == State.During)
            {
                EndGame();
                Win(); // TODO
            }
        }
        else if (enemyCount == 0)
        {
            if (combatState == State.During)
            {
                EndGame();
                Win();
            }
        }
    }

    private void EndGame()
    {
        combatState = State.After;
    }

    private void Win()
    {
        unitInfo.UpdateInfo(reward);
        unitInfoFade.Show();

        victoryText.text = $"{reward.Name} was like super impressed by your party's prowess. Let {reward.Name} join your team?";
        victoryFade.Show();
    }

    public void RecruitUnit()
    {
        reward.startPosition = Vector2.zero;
        giveReward = true;
    }

    public void ExitCombat()
    {
        foreach (UnitScript unit in listAllUnits)
        {
            Destroy(unit);
        }

        SceneManager.LoadScene(OverworldScene);
    }

    public void SetupCombat(List<Creature> playerTeam, List<Creature> enemyTeam)
    {
        listUnits = new List<UnitScript>();
        combatState = State.Before;

        foreach (Creature creature in playerTeam)
        {
            if (creature != null)
            {
                // We need to create a Sprite, and put the creature in that sprite
                GameObject newGuy = Instantiate(combatantPrefab);
                UnitScript unit = newGuy.GetComponent<UnitScript>();

                newGuy.AddComponent<Draggable>();

                unit.stats = creature;
                unit.team = false;

                Debug.Log(unit.stats + " " + unit.stats.startPosition);
                if (unit.stats.startPosition == Vector2.zero)
                {
                    unit.transform.position = new Vector2(Random.Range(-6f, -2f), Random.Range(-4f, 4f));
                }
                else
                {
                    unit.transform.position = unit.stats.startPosition;
                }

                unit.gameObject.layer = 10;

                unit.OnSpawned();

                listUnits.Add(unit);
            }
        }

        foreach (Creature creature in enemyTeam)
        {
            // We need to create a Sprite, and put the creature in that sprite
            GameObject newGuy = Instantiate(combatantPrefab);
            UnitScript unit = newGuy.GetComponent<UnitScript>();

            unit.stats = creature;
            unit.team = true;
            unit.transform.position = new Vector2(Random.Range(2f, 6f), Random.Range(-4f, 4f));

            unit.gameObject.layer = 11;

            unit.OnSpawned();

            listUnits.Add(unit);
        }

        reward = enemyTeam[0];

        listAllUnits = new List<UnitScript>(listUnits);

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
            Creature.GetBasicCreature(Creature.GetRandomEnumValue<Creature.BasicCreature>()),
            Creature.GetBasicCreature(Creature.GetRandomEnumValue<Creature.BasicCreature>()),
            Creature.GetBasicCreature(Creature.GetRandomEnumValue<Creature.BasicCreature>()),
        };
    }
    public static void StartCoroutineUsingManager(IEnumerator coroutine)
    {
        Instance.StartCoroutine(coroutine);
    }
}