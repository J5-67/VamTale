using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    PlayerController player;
    public float t;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                t += Time.deltaTime;

                if (t > speed)
                {
                    t = 0f;
                    Fire();
                }
                break;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
        {
            Batch();
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;
        for(int i = 0; i < GameManager.instance.poolManager.prefabs.Length; i++)
        {
            if(data.projectile == GameManager.instance.poolManager.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            case 1:
                speed = 0.3f;
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Batch()
    {
        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.poolManager.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<BulletController>().Init(damage, -100, Vector3.zero);
        }
    }

    public void Fire()
    {
        if (!player.sc.nearestTarget)
        {
            return;
        }

        Vector3 targetPos = player.sc.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        bullet.GetComponent<BulletController>().Init(damage, count, dir);
    }
}
