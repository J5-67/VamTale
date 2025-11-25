using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("아이템 정보")]
    public ItemData data;
    public int level;
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    PlayerController player;
    private float t;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (data.itemType)
        {
            case ItemType.Melee:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            case ItemType.Range:
                t += Time.deltaTime;
                if (t > speed)
                {
                    t = 0f;
                    Fire();
                }
                break;

            case ItemType.Lightning:
                t += Time.deltaTime;
                if (t > speed)
                {
                    t = 0f;
                    Meteor();
                }
                break;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (data.itemType == ItemType.Melee)
            Batch();

        level++;
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        this.data = data;
        this.level = 0;
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for (int i = 0; i < GameManager.instance.poolManager.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.poolManager.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (data.itemType)
        {
            case ItemType.Melee:
                speed = 150;
                Batch();
                break;
            case ItemType.Range:
                speed = 0.5f;
                break;
            case ItemType.Lightning:
                speed = 0.8f;
                break;
            default:
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
                bullet = transform.GetChild(i);
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

            bullet.GetComponent<BulletController>().Init(damage, -1, Vector3.zero);
        }
    }

    public void Fire()
    {
        if (!player.sc.nearestTarget) return;

        Transform target = player.sc.nearestTarget;
        Vector3 targetPos = target.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        bullet.GetComponent<BulletController>().Init(damage, count, dir, 15f);
    }

    public void Meteor()
    {
        if (!player.sc.farthestTarget) return;

        Transform target = player.sc.farthestTarget;
        Vector3 impactPos = target.position;
        Vector3 spawnPos = impactPos + new Vector3(0, 10, 0);

        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.position = spawnPos;
        bullet.rotation = Quaternion.identity;

        Vector3 dir = (impactPos - spawnPos).normalized;
        bullet.GetComponent<BulletController>().Init(damage, count, dir, 15f);
    }

    public void Evolve(ItemData data)
    {
        this.data = data;
        name = "Weapon " + data.itemId;
        this.damage = data.baseDamage;
        this.count = data.baseCount;
        this.id = data.itemId;

        for (int i = 0; i < GameManager.instance.poolManager.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.poolManager.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        if (data.itemType == ItemType.Melee)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
                child.parent = GameManager.instance.poolManager.transform;
            }
            Batch();
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}