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

    // [유니 추가] 스타트 화면(버튼 포함한 그룹)을 끄기 위해 변수 추가!
    public GameObject startGroup;

    [Header("플레이어 정보")]
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 7, 9, 150, 210, 280, 360, 450, 600 };

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("IsRestart") == 1)
        {
            PlayerPrefs.SetInt("IsRestart", 0);
            PlayerPrefs.Save();

            GameStart();
        }
    }

    public void GameStart()
    {
        health = maxHealth;

        if (startGroup != null)
        {
            startGroup.SetActive(false);
        }

        uiLevelUp.Select(0);
        isLive = true;
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

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
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