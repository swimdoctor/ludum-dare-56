using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealProjectile : Projectile
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
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (target == null || target.currentHP <= 0)
        {
            target = source.FindNewTarget();

        }
        if (target == null || target.currentHP <= 0)
        {
            isActive = false;
            StartCoroutine(Despawn());
        }
        else if (isActive)
        {
            rb.MovePosition(Vector2.MoveTowards((new Vector2(rb.position.x, rb.position.y)), target.transform.position, speed));
        }
    }
}
