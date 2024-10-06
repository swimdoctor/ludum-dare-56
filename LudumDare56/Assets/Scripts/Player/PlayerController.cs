using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    private Vector2 inputMovement;

    private PlayerInputActions playerInputActions;

    [SerializeField] private LayerMask interactableLayerMask;

    [SerializeField] private float interactRange;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        SubscribeInputActions();
        SwitchActionMap("World");
        print("enabled");
    }

    private void OnDisable()
    {
        UnSubscribeInputActions();
        SwitchActionMap();
		print("disabled");
	}

    private void SubscribeInputActions()
	{
		print("like and subscribe");
		playerInputActions.World.Move.started += MoveAction;
        playerInputActions.World.Move.performed += MoveAction;
        playerInputActions.World.Move.canceled += MoveAction;
		
		playerInputActions.World.Interact.performed += InteractAction;
    }

    private void UnSubscribeInputActions()
    {
        playerInputActions.World.Move.started -= MoveAction;
        playerInputActions.World.Move.performed -= MoveAction;
        playerInputActions.World.Move.canceled -= MoveAction;

        playerInputActions.World.Interact.performed -= InteractAction;
    }

    private void MoveAction(InputAction.CallbackContext context)
    {
		print("Be sure to hit that bell!");
		inputMovement = context.ReadValue<Vector2>();
        playerMovement.SetMovement(inputMovement);
    }

    //Action Callback when the player wants to interact with an object
    private void InteractAction(InputAction.CallbackContext context)
    {
        Vector2 direction = new Vector2();
        Direction direc = playerMovement.GetDirection();
        switch (direc)
        {
            case (Direction.Up):
                Debug.Log("Checking up");
                direction = Vector2.up;
                break;
            case (Direction.Down):
                Debug.Log("Checking Down");
                direction = Vector2.down;
                break;
            case(Direction.Left):
                Debug.Log("Checking Left");
                direction = Vector2.left;
                break;
            case (Direction.Right):
                Debug.Log("Checking Right");
                direction = Vector2.right;
                break;
        }

        //Find an object in the raycast

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, interactRange, interactableLayerMask);

        //If it has correct properties, then call its Interact function
        if(hit2D.collider != null)
        {
            IInteractable obj = hit2D.collider.GetComponent(typeof(IInteractable)) as IInteractable;
            if (obj != null)
            {
                //TODO:
                //Call function that triggers action within the object
                Debug.Log("Found valid Object");
                obj.Interact(gameObject);
            }
            else
            {
                Debug.Log("Invalid Object");
            }
        }
        else
        {
            Debug.Log("Object is Null");
        }
        

    }

    //Update Current ActionMap
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
