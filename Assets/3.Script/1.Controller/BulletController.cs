using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rg;

    private void Awake()
    {
        TryGetComponent(out rg);
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0)
        {
            rg.linearVelocity = dir * 15f;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100)
        {
            return;
        }

        per--;

        if (per < 0)
        {
            rg.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
