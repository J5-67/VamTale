using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("게임 매니저")]
    [SerializeField] public PlayerController player;
    public PoolManager poolManager;
    public LevelUp uiLevelUp;
    public GameObject startGroup;
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("플레이어 정보")]
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 7, 9, 150, 210, 280, 360, 450, 600 };

    [Header("연출")]
    public Image fadePanel;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
            fadePanel.color = new Color(1, 1, 1, 0);
        }

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

        if (gameTime >= maxGameTime)
        {
            gameTime = maxGameTime;
            StartCoroutine(ToBattleScene_co());
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

    IEnumerator ToBattleScene_co()
    {
        isLive = false;

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float duration = 2.0f;
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = t / duration;
                fadePanel.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            fadePanel.color = Color.white;
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        SceneManager.LoadScene("Battle");
    }
}