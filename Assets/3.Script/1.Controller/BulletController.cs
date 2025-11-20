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

        if(per > -1)
        {
            rg.linearVelocity = dir * 15f;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy") || per == -1)
        {
            return;
        }

        per--;

        if(per == -1)
        {
            rg.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
