using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOver;

    [SerializeField]
    private GameObject heart;

    [SerializeField]
    private GameObject heartBreak;

    [SerializeField]
    private List<GameObject> heartShards = new List<GameObject>();

    [SerializeField]
    private float power;

    [SerializeField]
    private float typingSpeed = 0.5f;

    [SerializeField]
    private TMP_Text gameOverText;

    private SpriteRenderer gameOverColor;

    private void Start()
    {
        if(gameOver != null)
        {
            if(!gameOver.TryGetComponent(out gameOverColor))
            {
                Debug.LogError("[GameOver.cs] gameOver GameObject에 SpriteRenderer 컴포넌트가 없습니다! 확인해주세요.");
            }
        }
        else
        {
            Debug.LogError("[GameOver.cs] gameOver 변수가 Inspector에 연결되지 않았습니다! 확인해주세요.");
        }
        if(gameOverColor != null)
        {
            StartCoroutine(GameOver_co());
        }
        else
        {
            Debug.LogError("[GameOver.cs] 초기화 실패로 GameOver 코루틴을 시작할 수 없습니다.");
        }
    }

    private IEnumerator GameOver_co()
    {
        heart.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        heart.SetActive(false);
        heartBreak.SetActive(true);

        yield return new WaitForSeconds(1f);

        heartBreak.SetActive(false);
        for (int i = 0; i < heartShards.Count; i++)
        {
            heartShards[i].SetActive(true);

            GameObject shards = heartShards[i];

            Rigidbody2D rb = shards.GetComponent<Rigidbody2D>();

            Vector2 direction = ((Vector2)shards.transform.position - Vector2.zero).normalized;
            rb.AddForce(direction * power, ForceMode2D.Impulse);
            rb.AddTorque(UnityEngine.Random.Range(-5f, 5f));
        }

        yield return new WaitForSeconds(1.5f);

        gameOver.SetActive(true);

        Color startColor = gameOverColor.color;
        startColor.a = 0f;
        gameOverColor.color = startColor;

        float fadeDuration = 1.5f;
        float elapsed = 0f;

        while(elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            Color currentColor = gameOverColor.color;

            currentColor.a = Mathf.Lerp(0f, 1f, t);

            gameOverColor.color = currentColor;

            yield return null;
        }

        Color finalColor = gameOverColor.color;
        finalColor.a = 1f;
        gameOverColor.color = finalColor;

        string text = gameOverText.text;

        yield return StartCoroutine(TextWriter_co(text));
    }

    private IEnumerator TextWriter_co(string text)
    {
        gameOverText.text = "";

        RectTransform textRect = gameOverText.rectTransform;

        textRect.anchoredPosition = new Vector2(100f, -100f);

        foreach (char c in text)
        {
            gameOverText.text += c;

            //사운드 추가

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
