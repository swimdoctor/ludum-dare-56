using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected UnitScript source;
    protected Vector2 direction;
    protected BasicAttack attack;

    protected bool isActive;

    protected float lifetime = 2f; // Delete projectile after this many seconds

    protected int pierce = 1; // Number of enemies

    protected bool isHealProjectile;
    protected bool targetTeam;

    protected UnitScript target;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    public virtual void Initialize(UnitScript source, Vector2 direction, BasicAttack attack, bool isHealProjectile, UnitScript target)
    {
        Debug.Log("Initing");
        this.source = source;
        this.direction = direction;
        this.attack = attack;
        this.isHealProjectile = isHealProjectile;
        this.target = target;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (isHealProjectile)
        {
            targetTeam = source.team;
        }
        else
        {
            targetTeam = !source.team;
        }
        isActive = true;
    }

    protected virtual void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime < 0)
        {
            isActive = false;
            StartCoroutine(Despawn());
        }
    }

    protected virtual void Hit(UnitScript target)
    {
        float damage = attack.calcDamage(source, ranged: true);
        target.ChangeHP(-damage);

        pierce -= 1;
        if (pierce <= 0)
        {
            isActive = false;
            StartCoroutine(Despawn());
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.gameObject.layer == 10)
            {
                if (!targetTeam)
                {
                    Hit(collision.gameObject.GetComponent<UnitScript>());
                }
            }
            else if (collision.gameObject.layer == 11)
            {
                if (targetTeam)
                {
                    Hit(collision.gameObject.GetComponent<UnitScript>());
                }
            }
        }
    }

    protected IEnumerator Despawn()
    {
        Color color = sr.color;
        float startAlpha = color.a;
        float despawnTime = 0.125f;

        for (float t = 0; t < despawnTime; t += Time.deltaTime)
        {
            float normalizedTime = t / despawnTime;
            color.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            sr.color = color;
            yield return null;
        }
        color.a = 0;
        sr.color = color;
        Destroy(gameObject);
    }
}
