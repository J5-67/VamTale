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
        {
            return;
        }

        // [유니 수정] id(숫자) 대신 data.itemType(종류)으로 행동을 결정하게 바꿨어!
        // 이제 진화해서 ID가 바뀌어도 'Melee' 타입이기만 하면 계속 뱅글뱅글 돌 거야.
        switch (data.itemType)
        {
            case ItemType.Melee: // 근접 무기 (회전)
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default: // 그 외 (발사)
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

        level++;

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        this.data = data;
        this.level = 0;

        //Property Set
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

        // [유니] 초기화할 때는 id를 써도 괜찮아 (어차피 처음이니까)
        // 하지만 나중에 data.itemType으로 통일하면 더 좋긴 해!
        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
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
            bullet.GetComponent<BulletController>().Init(damage, -100, Vector3.zero); // -100 is Infinity Per
        }
    }

    public void Fire()
    {
        if (!player.sc.nearestTarget)
        {
            return;
        }

        Transform target = player.sc.nearestTarget;

        if (data.itemType == ItemType.Lightning)
        {
            target = player.sc.farthestTarget;
        }

        if (target == null)
            return;

        Vector3 targetPos = target.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;

        if (data.itemType == ItemType.Lightning)
        {
            bullet.position = targetPos;
            bullet.rotation = Quaternion.identity;

            bullet.GetComponent<BulletController>().Init(damage, count, Vector3.zero);
        }
        else
        {
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<BulletController>().Init(damage, count, dir);
        }
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

        // [유니 수정] 진화했을 때 근접 무기라면, 기존 헌 무기들은 치우고 새 무기를 깔아줘야 해!
        if (data.itemType == ItemType.Melee)
        {
            // 1. 기존에 돌고 있던 자식들(옛날 무기)을 모두 비활성화하고 부모에서 떼어내기
            // (역순으로 해야 에러가 안 나!)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(false);
                child.parent = GameManager.instance.poolManager.transform; // 풀 매니저로 돌려보내기
            }

            // 2. 새로운 무기 세팅!
            Batch();
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}