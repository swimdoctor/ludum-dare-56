using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager instance;

	public List<Creature> inventory = new List<Creature>();
	public List<Creature> bestiary = new List<Creature>();
	public List<Creature> party = new List<Creature>(new Creature[5]);

	public GameObject creaturePrefab;
	public GameObject partyMenu;

	[SerializeField] Sprite Kick;
	public Sprite Add;

	Transform[] partyLocations = new Transform[5];

	bool partySelected = true;
	int selectedIndex = -1;

	private void OnEnable()
	{
		instance = this; 
		for(int i = 0; i < 5; i++)
		{
			partyLocations[i] = partyMenu.transform.GetChild(1).GetChild(i);
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

		//Update InventoryMenuObject size
		Transform content = partyMenu.transform.GetChild(2).GetChild(0).GetChild(0);
		content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventory.Count * 50);
		while(content.childCount < inventory.Count)
		{
			GameObject button = new GameObject();
			button.AddComponent<RectTransform>().SetParent(content);
			button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, .5f);
			button.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
			button.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 50 * content.childCount - 50, 50);
			button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 32);
			int index = content.childCount - 1;
			button.name = "Creature " + index;
			button.AddComponent<Button>().onClick.AddListener(() => SelectInventory(index));
			button.AddComponent<Image>().color = new Color(0.3f, 0.8f, 0.6f, 0);
			GameObject GO = Instantiate(creaturePrefab, button.transform);
			GO.AddComponent<RectTransform>();
			GO.GetComponent<RectTransform>().localScale = new Vector3(10, 10, 1);
			GO.GetComponent<RectTransform>().anchoredPosition = new Vector2(.5f, .5f);

			UpdateInventorySprites();
		}
	}

	public void UpdateInventorySprites()
	{
		Transform content = partyMenu.transform.GetChild(2).GetChild(0).GetChild(0);
		for(int i = 0; i < Math.Min(inventory.Count, content.childCount); i++)
		{
			content.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = inventory[i].torso;
			content.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = inventory[i].leftLeg;
			content.GetChild(i).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = inventory[i].rightLeg;
			content.GetChild(i).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = inventory[i].leftArm;
			content.GetChild(i).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = inventory[i].rightArm;
			content.GetChild(i).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = inventory[i].head;
			content.GetChild(i).GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = inventory[i].headAccessory;
			SpriteSkinUtility.ResetBindPose(content.GetChild(i).GetChild(0).GetComponent<SpriteSkin>());
			SpriteSkinUtility.ResetBindPose(content.GetChild(i).GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
		}

		for(int i = inventory.Count; i < content.childCount; i++)
		{
			content.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = null;
			content.GetChild(i).GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = null;
		}
	}

	public void UpdateParty(Creature creature, int index)
	{
		if(index < 0 || index >= 5)
			return;

		party[index] = creature;

		if(partyLocations[index].childCount == 0)
			Instantiate(creaturePrefab, partyLocations[index]).transform.localScale = new Vector3(10, 10, 1);

		//Update Sprites
		if(creature == null)
		{
			partyLocations[index].GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = null;
			partyLocations[index].GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = null;
		}
		else
		{
			partyLocations[index].GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.torso;
			partyLocations[index].GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.leftLeg;
			partyLocations[index].GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = creature.rightLeg;
			partyLocations[index].GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = creature.leftArm;
			partyLocations[index].GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = creature.rightArm;
			partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = creature.head;
			partyLocations[index].GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = creature.headAccessory;
			SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetComponent<SpriteSkin>());
			SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
		}
		

		//Rebind Poses
		
	}

	public void SelectParty(int index)
	{
		partySelected = true;
		selectedIndex = index;
		UpdatePartyStats(party[index]);
	}
	public void SelectInventory(int index)
	{
		print(index);
		partySelected = false;
		selectedIndex = index;
		UpdatePartyStats(inventory[index]);
	}

	public void UpdatePartyStats(Creature creature)
	{
		Transform partyStatsTorsoTransform = partyMenu.transform.GetChild(0).GetChild(0);
		//Update Creature Icon
		if(partyStatsTorsoTransform.childCount == 0)
		{
			Instantiate(creaturePrefab, partyStatsTorsoTransform).transform.localScale = new Vector3(10, 10, 1);
		}

		if(creature == null)
		{
			partyStatsTorsoTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = null;
			partyStatsTorsoTransform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = null;
		}
		else
		{
			partyStatsTorsoTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.torso;
			partyStatsTorsoTransform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.leftLeg;
			partyStatsTorsoTransform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = creature.rightLeg;
			partyStatsTorsoTransform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = creature.leftArm;
			partyStatsTorsoTransform.GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = creature.rightArm;
			partyStatsTorsoTransform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = creature.head;
			partyStatsTorsoTransform.GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = creature.headAccessory;
			SpriteSkinUtility.ResetBindPose(partyStatsTorsoTransform.GetChild(0).GetComponent<SpriteSkin>());
			SpriteSkinUtility.ResetBindPose(partyStatsTorsoTransform.GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
		}
		

		//Update Creature Stats

		//Update Add/Kick Button
		if(party.Contains(creature))
			partyMenu.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Kick;
		else
			partyMenu.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Add;

	}

	public void PressAddKick()
	{
		if(selectedIndex < 0)
			return;

		if(partySelected && selectedIndex < 5)
		{
			UpdateParty(null, selectedIndex);
			UpdatePartyStats(null);
			selectedIndex = -1;
		}
	}
}
