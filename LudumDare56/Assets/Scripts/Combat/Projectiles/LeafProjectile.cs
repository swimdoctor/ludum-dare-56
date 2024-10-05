using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafProjectile : Projectile
{
    [SerializeField] private float speed = 0.1f;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, -90+Mathf.Rad2Deg*Mathf.Atan2(direction.y, direction.x));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        rb.MovePosition(rb.position + (direction * speed));
    }
}
