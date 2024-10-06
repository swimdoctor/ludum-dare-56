using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafProjectile : Projectile
{
    private float speed = 0.1f;

    public override void Initialize(UnitScript source, Vector2 direction, BasicAttack attack, bool isHealProjectile, UnitScript target)
    {
        base.Initialize(source, direction, attack, isHealProjectile, target);
        this.pierce = 1;
        this.lifetime = 2f;
        
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
