using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rigidbody;

    private Vector2 inputMovement;

    [SerializeField] private float moveSpeed;

    private Direction direction;

    public Direction GetDirection()
    {
        return direction;
    } 

    private void Awake()
    {
        direction = Direction.Up;
    }
    private void FixedUpdate()
    {


        DecideFacingDirection();

        MoveCharacter();

        
    }

    //Update the input Vector for MoveCharacter
    public void SetMovement(Vector2 movement)
    {
        inputMovement = movement;
    }

    //Move the character in the scene based on inputMovement
    private void MoveCharacter()
    {
        Vector2 movement = new Vector2();

        if (Mathf.Abs(inputMovement.x) > 0.01f)
        {
            movement.x = inputMovement.x*moveSpeed;
        }

        if (Mathf.Abs(inputMovement.y) > 0.01f)
        {
            movement.y = inputMovement.y*moveSpeed;
        }

        movement *= Time.deltaTime;

        rigidbody.MovePosition(rigidbody.position+movement);


    }

    //Figure out direction last movement was in and sets direction
    private void DecideFacingDirection()
    {
        if (inputMovement.x == inputMovement.y)
        {
            return;
        }

        if (Mathf.Abs(inputMovement.x) > Mathf.Abs(inputMovement.y))
        {
            if (inputMovement.x > 0)
            {
                direction = Direction.Right;
            }
            else
            {
                direction = Direction.Left;
            }
        }
        else
        {
            if (inputMovement.y > 0)
            {
                direction = Direction.Up;
            }
            else
            {
                direction = Direction.Down;
            }
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
