using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class StartCombatButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Button");
        CombatManager.Instance.StartCombat();
    }
}