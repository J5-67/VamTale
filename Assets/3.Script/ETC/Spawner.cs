using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public int spriteType;
    public int health;
    public float spawnTime;
    public float speed;
}

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float t;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }
    private void Update()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        t += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);

        //if(level >= spawnData.Length)
        //{
        //    level = spawnData.Length - 1;
        //}

        level = Mathf.Min(level, spawnData.Length - 1);

        if (t > spawnData[level].spawnTime)
        {
            t = 0f;
            Spawn();
        }
    }

    public void Spawn()
    {
        GameObject enemy = GameManager.instance.poolManager.Get(0);
        enemy.transform.position = spawnPoint[UnityEngine.Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<EnemyController>().Init(spawnData[level]);

    }
}
