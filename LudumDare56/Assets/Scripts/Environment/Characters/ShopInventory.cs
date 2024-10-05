using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class ShopInventory : MonoBehaviour, IInteractable
{
    [SerializeField] List<ShopItem> items;

    //Use with Shop UI
    //[SerializeField] UIDocument shopMenu;

    public void Interact()
    {

        //TODO: Called when Shop keeper is interacted with
    }

    public void OpenShop()
    {

        //TODO: Pause Game
        //TODO: Open shopMenu
    }

    public void CloseShop() 
    {
        //TODO: Resume Game
        //TODO: Close shopMenu
    }

    public void Interact(GameObject user)
    {
        Debug.Log("Interacted with shoppe");
    }
}

[System.Serializable]
struct ShopItem
{
    [SerializeField] private string itemID;
    [SerializeField] private int itemCost;
    [SerializeField] private int itemStock;

    public ShopItem(string itemID="", int itemCost = 0, int stock=0)
    {
        this.itemID = itemID;
        this.itemCost = itemCost;
        this.itemStock = stock;
    }
}
