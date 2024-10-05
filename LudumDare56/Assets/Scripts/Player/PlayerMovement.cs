using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rigidbody;

    private Vector2 inputMovement;

    [SerializeField] private float moveSpeed;



    private void FixedUpdate()
    {
        MoveCharacter();
    }

    public void SetMovement(Vector2 movement)
    {
        inputMovement = movement;
    }

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




}
