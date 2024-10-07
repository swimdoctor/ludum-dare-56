using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager instance;

	public static List<Creature> inventory = new List<Creature>();
	public static List<Creature> bestiary = new List<Creature>();
	public static List<Creature> party = new List<Creature>(new Creature[5]);
	public Creature mergeA;
	public Creature mergeB;
	public Creature mergeAB;

	public GameObject creaturePrefab;

	[SerializeField] Sprite Kick;
	public Sprite Add;

	Transform[] partyLocations = new Transform[5];
	Transform[] mergeLocations = new Transform[3];

	bool partySelected = true;
	int selectedIndex = -1;

	private void OnEnable()
	{
		instance = this; 

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name} with mode: {mode}");
        // You can add your logic here for what to do when the scene is loaded

		if (CombatManager.giveReward)
		{
			StartCoroutine(giveNewCreature());
		}
    }

	IEnumerator giveNewCreature()
	{
		yield return null;
        AddCreature(CombatManager.reward);
        CombatManager.giveReward = false;
    }

    // Start is called before the first frame update
    void Start()
	{
		for(int i = 0; i < 5; i++)
		{
			partyLocations[i] = OOBMenu.instance.party.transform.GetChild(0).GetChild(i);
		}
		for(int i = 0; i < 3; i++)
		{
			mergeLocations[i] = OOBMenu.instance.merge.transform.GetChild(0).GetChild(i);
		}
		if(inventory.Count == 0)
		{
			AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Plant));
			AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Knight));
			AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Steampunk));
			AddCreature(Creature.GetBasicCreature(Creature.BasicCreature.Burger));
		}
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
		Transform content = OOBMenu.instance.inventory.transform.GetChild(0).GetChild(0);
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
		}
		UpdateInventorySprites();
	}

	//Player loses a creature due to merging
	public void RemoveCreature(Creature creature)
	{
		bestiary.Remove(creature);
		inventory.Remove(creature);
		for(int i = 0; i < 5; i++)
		{
			if(party[i] == creature)
			{
				UpdateParty(null, i);
			}
		}

		//Update InventoryMenuObject size
		Transform content = OOBMenu.instance.inventory.transform.GetChild(0).GetChild(0);
		content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventory.Count * 50);
		UpdateInventorySprites();
	}

	public void UpdateInventorySprites()
	{
		Transform content = OOBMenu.instance.inventory.transform.GetChild(0).GetChild(0);
		for(int i = 0; i < Math.Min(inventory.Count, content.childCount); i++)
		{
			content.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = inventory[i].torso;
			content.GetChild(i).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = inventory[i].leftLeg;
			content.GetChild(i).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = inventory[i].rightLeg;
			content.GetChild(i).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = inventory[i].leftArm;
			content.GetChild(i).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = inventory[i].rightArm;
			content.GetChild(i).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = inventory[i].head;
			content.GetChild(i).GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = inventory[i].headAccessory;
			try
			{
				SpriteSkinUtility.ResetBindPose(content.GetChild(i).GetChild(0).GetComponent<SpriteSkin>());
				SpriteSkinUtility.ResetBindPose(content.GetChild(i).GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
			}
			catch(InvalidOperationException)
			{
				print("lol take that spriteSkin");
			}
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
		//print((creature==null) + " " + (party[index]==null));

		if(partyLocations[index].childCount == 0)
			Instantiate(creaturePrefab, partyLocations[index]).transform.localScale = new Vector3(10, 10, 1);
		UpdateStats(creature);
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
			//print("Set index" + index + "'s sprites to null");
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
			//print(creature.torso);
			//print(index);
			//for(int i = 0; i < 5; i++)
			//	if(party[i] != null)
			//		print(i + party[i].Name);
			try 
			{ 
				SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetComponent<SpriteSkin>());
				SpriteSkinUtility.ResetBindPose(partyLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
			}
			catch(InvalidOperationException)
			{
				print("lol take that spriteSkin");
			}
		}
	}

	public void UpdateMerge(Creature creature, int index)
	{
		if(index < 0 || index > 2)
			return;

		if(index == 0)
			mergeA = creature;
		else if(index == 1)
			mergeB = creature;
		else
			mergeAB = creature;

		if(mergeLocations[index].childCount == 0)
			Instantiate(creaturePrefab, mergeLocations[index]).transform.localScale = new Vector3(10, 10, 1);
		UpdateStats(creature);
		//Update Sprites
		if(creature == null)
		{
			mergeLocations[index].GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = null;
			mergeLocations[index].GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = null;
		}
		else
		{
			mergeLocations[index].GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.torso;
			mergeLocations[index].GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = creature.leftLeg;
			mergeLocations[index].GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = creature.rightLeg;
			mergeLocations[index].GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = creature.leftArm;
			mergeLocations[index].GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().sprite = creature.rightArm;
			mergeLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sprite = creature.head;
			mergeLocations[index].GetChild(0).GetChild(5).GetComponent<SpriteRenderer>().sprite = creature.headAccessory;
			try
			{
				SpriteSkinUtility.ResetBindPose(mergeLocations[index].GetChild(0).GetComponent<SpriteSkin>());
				SpriteSkinUtility.ResetBindPose(mergeLocations[index].GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
			}
			catch(InvalidOperationException)
			{
			print("lol take that spriteSkin");
			}
		}
	}

	public void SelectParty(int index)
	{
		partySelected = true;
		selectedIndex = index;
		UpdateStats(party[index]);
	}

	public void SelectInventory(int index)
	{
		partySelected = false;
		selectedIndex = index;
		UpdateStats(inventory[index]);
	}

	public void MergeButton()
	{
		if(mergeA == null || mergeB == null)
			return;
		Creature AB = Creature.Merge(mergeA, mergeB);
		RemoveCreature(mergeA);
		RemoveCreature(mergeB);
		UpdateMerge(null, 0);
		UpdateMerge(null, 1);

		AddCreature(AB);
		UpdateMerge(AB, 2);
	}

	internal void ClearMerge()
	{
		UpdateMerge(null, 0);
		UpdateMerge(null, 1);
		UpdateMerge(null, 2);
	}

	public void UpdateStats(Creature creature)
	{
		Transform partyStatsTorsoTransform = OOBMenu.instance.stats.transform.GetChild(0);
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
			try
			{
				SpriteSkinUtility.ResetBindPose(partyStatsTorsoTransform.GetChild(0).GetComponent<SpriteSkin>());
				SpriteSkinUtility.ResetBindPose(partyStatsTorsoTransform.GetChild(0).GetChild(4).GetComponent<SpriteSkin>());
			}
			catch(InvalidOperationException)
			{
				print("lol take that spriteSkin");
			}
		}	
		

		//Update Creature Stats ----------------------------------------------

		//Update Add/Kick Button
		if(party.Contains(creature))
			OOBMenu.instance.stats.transform.GetChild(1).GetComponent<Image>().sprite = Kick;
		else
			OOBMenu.instance.stats.transform.GetChild(1).GetComponent<Image>().sprite = Add;

	}

	public void PressAddKickMerge()
	{
		if(selectedIndex < 0)
			return;

		if(OOBMenu.instance.menuState == OOBMenu.MenuState.Party)
		{
			if(partySelected && selectedIndex < 5)
			{
				UpdateParty(null, selectedIndex);
			}
			else if(!partySelected && selectedIndex < inventory.Count)
			{
				if(!party.Contains(inventory[selectedIndex])){
					for(int i = 0; i < 5; i++)
					{
						if(party[i] == null)
						{
							UpdateParty(inventory[selectedIndex], i);
							break;
						}
					}
				}
				else
				{
					UpdateParty(null, party.IndexOf(inventory[selectedIndex]));
				}
			}
		}
		else if(OOBMenu.instance.menuState == OOBMenu.MenuState.Merge)
		{
			if(partySelected)
			{
				if(selectedIndex < 2)
				{
					UpdateMerge(null, selectedIndex);
				}
				if(selectedIndex == 2)
				{
					if(mergeA == null)
						UpdateMerge(mergeAB, 0);
					else if(mergeB == null)
						UpdateMerge(mergeAB, 1);
					if(mergeAB != null) UpdateMerge(null, 2);
				}
			}
			else if(!partySelected && selectedIndex < inventory.Count)
			{
				Creature creature = inventory[selectedIndex];
				if(creature != mergeA && creature != mergeB)
				{
					if(mergeA == null)
						UpdateMerge(creature, 0);
					else if(mergeB == null)
						UpdateMerge(creature, 1);
					if(mergeAB != null)UpdateMerge(null, 2);
				}
				else
				{
					if(mergeA == creature)
					{
						UpdateMerge(null, 0);
						mergeA = null;
					}
					if(mergeB == creature)
					{
						UpdateMerge(null, 0);
						mergeB = null;
					}
				}
			}
		}
	}
}
