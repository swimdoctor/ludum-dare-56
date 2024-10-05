using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafProjectile : Projectile
{
    [SerializeField] private float speed = 0.5f;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        rb.MovePosition(direction * speed);
    }
}
