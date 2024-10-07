using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager instance;

	public List<Creature> inventory = new List<Creature>();
	public List<Creature> bestiary = new List<Creature>();
	public Creature[] party = new Creature[5];

	public GameObject creaturePrefab;
	public GameObject partyMenuCreatures;

	[SerializeField] Sprite Kick;
	public Sprite Add;

	Transform[] partyLocations = new Transform[5];

	private void OnEnable()
	{
		instance = this; 
		for(int i = 0; i < 5; i++)
		{
			partyLocations[i] = partyMenuCreatures.transform.GetChild(i);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Plant));
        AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Knight));
        AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Steampunk));
        AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Burger));
    }

	//Player collects new creature
	public void AddCreature(Creature creature)
	{
		bestiary.Add(creature);
		inventory.Add(creature);

		//Creature enters party if empty space
		for(int i = 0; i < 5; i++)
		{
			if(party[i] == null) 
			{
				UpdateParty(creature, i);
				break;
			}
		}
	}

	public void UpdateParty(Creature creature, int index)
	{
		if(index < 0 || index >= 5)
			return;

		party[index] = creature;

		if(partyLocations[index].childCount == 0)
			Instantiate(creaturePrefab, partyLocations[index]).transform.localScale = new Vector3(30, 30, 1);

		//Update Sprites
		partyLocations[index].GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.torso;
		partyLocations[index].GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.leftLeg;
		partyLocations[index].GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = creature.rightLeg;
		partyLocations[index].GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = creature.leftArm;
		partyLocations[index].GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = creature.rightArm;
		partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = creature.head;
		partyLocations[index].GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = creature.headAccessory;

		//Rebind Poses
	// 	SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetComponent<SpriteSkin>());
	// 	SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
	}

// 	public void UpdatePartyStats(Creature creature)
// 	{
// 		//Update Creature Icon

// 		//Update Creature Stats
// 		//Update Add/Kick Button
// 	}
}
