using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public UnitScript source;
    public Vector2 direction;
    public BasicAttack attack;

    public bool isActive;

    protected float lifetime = 2f; // Delete projectile after this many seconds

    public int pierce = 1; // Number of enemies

    public bool isHealProjectile;
    public bool targetTeam;

    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    public void OnSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (isHealProjectile)
        {
            targetTeam = source.team;
        } else
        {
            targetTeam = !source.team;
            Debug.Log(source.team);
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
        Debug.Log(pierce);
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
            color.a = Mathf.Lerp(startAlpha, 0, normalizedTime); // Fade to transparent
            sr.color = color;
            yield return null; // Wait for the next frame
        }
        color.a = 0; // Ensure it's fully transparent at the end
        sr.color = color;
        Destroy(gameObject);
    }
}
