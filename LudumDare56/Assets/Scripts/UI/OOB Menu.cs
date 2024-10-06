using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OOBMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    private bool isMenuActive = false;

    void Start()
    {
        GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        GetComponent<Image>().enabled = isMenuActive;
    }
}