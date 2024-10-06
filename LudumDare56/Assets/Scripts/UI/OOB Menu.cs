using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;


public class OOBMenu : MonoBehaviour
{
    public static OOBMenu instance;
    public enum MenuState
    {
        None,
        Menu,
        Party,
        Bag,
        Merge,
        Settings
    }

    public MenuState menuState = MenuState.None;

    public GameObject menu;
    public GameObject party;
    public GameObject bag;

	private void OnEnable()
	{
		instance = this;
	}

	private void Start()
	{
        UpdateMenu();
	}

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if(menuState == MenuState.None)
                SwapState(1);
            else
                SwapState(0);
		}
    }

    public void UpdateMenu()
    {
        switch(menuState)
        {
            case MenuState.None:
                menu.SetActive(false);
                party.SetActive(false);
                bag.SetActive(false);
                //Close other menus
                break;
            case MenuState.Menu:
				menu.SetActive(true);
				party.SetActive(false);
				bag.SetActive(false);
				//Close other menus
				break;
            case MenuState.Party:
				menu.SetActive(true);
				party.SetActive(true);
				bag.SetActive(false);
				//Close other menus
				break;
            case MenuState.Bag:
				menu.SetActive(true);
				party.SetActive(false);
				bag.SetActive(true);
				//Close other menus
				break;
            case MenuState.Settings:
				menu.SetActive(true);
				party.SetActive(false);
				bag.SetActive(false);
				//OpenSettingsMenu
				//Close other menus
				break;
		}
    }

    public void SwapState(int state)
    {
        menuState = (MenuState)state;
        UpdateMenu();
	}
}