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
    public float levelTime;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }
    private void Update()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        t += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

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
