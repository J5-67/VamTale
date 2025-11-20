using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("게임 매니저")]
    [SerializeField] public PlayerController player;
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public PoolManager poolManager;
    public LevelUp uiLevelUp;
    [Header("플레이어 정보")]
    public int health;
    public int maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 7, 9, 150, 210, 280, 360, 450, 600 };

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        health = maxHealth;

        uiLevelUp.Select(0);
    }

    private void Update()
    {
        if (!isLive)
        {
            return;
        }

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    public void GetExp()
    {
        exp++;

        if(exp == nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

}
