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
        TryGetComponent<Rigidbody2D>(out rg);
    }

    public void Init(float damage, int per, Vector3 dir, float speed = 15f)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)
        {
            if (rg != null)
            {
                rg.linearVelocity = dir * speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if (per < 0)
        {
            rg.linearVelocity = Vector2.zero;

            StartCoroutine(DisableBullet());
        }
    }

    IEnumerator DisableBullet()
    {
        yield return new WaitForFixedUpdate();

        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -1)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
