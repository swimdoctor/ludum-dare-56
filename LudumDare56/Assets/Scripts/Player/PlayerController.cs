using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    private Vector2 inputMovement;

    private PlayerInputActions playerInputActions;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

    }

    private void OnEnable()
    {
        SubscribeInputActions();
        SwitchActionMap("World");

    }

    private void OnDisable()
    {
        UnSubscribeInputActions();
        SwitchActionMap();
    }

    private void SubscribeInputActions()
    {
        playerInputActions.World.Move.started += MoveAction;
        playerInputActions.World.Move.performed += MoveAction;
        playerInputActions.World.Move.canceled += MoveAction;
    }

    private void UnSubscribeInputActions()
    {
        playerInputActions.World.Move.started -= MoveAction;
        playerInputActions.World.Move.performed -= MoveAction;
        playerInputActions.World.Move.canceled -= MoveAction;
    }

    private void MoveAction(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
        playerMovement.SetMovement(inputMovement);
    }

    private void SwitchActionMap(string mapName = "")
    {
        playerInputActions.World.Disable();

        switch (mapName)
        {
            case "World":
                playerInputActions.World.Enable();
                break;

            default:
                Debug.Log("Improper InputAction Map");
                break;
        }
    }




}
