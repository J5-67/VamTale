using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("--- Game Over Objects ---")]
    [SerializeField]
    private GameObject gameOver;

    [SerializeField]
    private GameObject heart;

    [SerializeField]
    private GameObject heartBreak;

    [SerializeField]
    private List<GameObject> heartShards = new List<GameObject>();

    [Header("--- Settings ---")]
    [SerializeField]
    private float power;

    [SerializeField]
    private float typingSpeed = 0.1f;

    [TextArea]
    public string dialogueMessage = "You cannot give up just yet...\nDon't lose hope!\nStay determined!";

    [Header("--- UI ---")]
    [SerializeField]
    private TMP_Text gameOverText;

    [SerializeField]
    private Button giveup;
    [SerializeField]
    private TMP_Text giveupText;
    [SerializeField]
    private Button notgiveup;
    [SerializeField]
    private TMP_Text notgiveupText;

    private SpriteRenderer gameOverColor;

    private void Start()
    {
        if (giveup != null)
            giveup.onClick.AddListener(OnClickGiveUp);

        if (notgiveup != null)
            notgiveup.onClick.AddListener(OnClickNotGiveUp);

        if (gameOverText != null) gameOverText.text = "";
        if (giveup != null) giveup.gameObject.SetActive(false);
        if (notgiveup != null) notgiveup.gameObject.SetActive(false);

        if (gameOver != null)
        {
            if (gameOver.TryGetComponent(out gameOverColor))
            {
                StartCoroutine(GameOver_co());
            }
            else
            {
                Debug.LogError("[GameOver.cs] SpriteRenderer가 없습니다!");
            }
        }
    }

    public void OnClickGiveUp()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit(); // 실제 빌드된 게임에서는 이게 꺼주는 거야!
        #endif
    }

    public void OnClickNotGiveUp()
    {
        StartCoroutine(StayDetermined_co());
    }

    private IEnumerator StayDetermined_co()
    {
        gameOver.SetActive(false);
        giveup.gameObject.SetActive(false);
        notgiveup.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        giveupText.gameObject.SetActive(false);
        notgiveupText.gameObject.SetActive(false);

        heartBreak.SetActive(true);
        heart.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        heartBreak.SetActive(false);
        heart.SetActive(true);

        // [유니 꿀팁] 여기에 '챙!' 하는 효과음 넣으면 진짜 멋있어!
        // AudioManager.instance.Play("Heal"); 

        yield return new WaitForSeconds(1.5f);

        PlayerPrefs.SetInt("IsRestart", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainGame");
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
            if (heartShards[i].TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                Vector2 direction = ((Vector2)heartShards[i].transform.position - Vector2.zero).normalized;
                rb.AddForce(direction * power, ForceMode2D.Impulse);
                rb.AddTorque(UnityEngine.Random.Range(-5f, 5f));
            }
        }

        yield return new WaitForSeconds(1.5f);

        gameOver.SetActive(true);
        Color startColor = gameOverColor.color;
        startColor.a = 0f;
        gameOverColor.color = startColor;

        float fadeDuration = 1.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            Color currentColor = gameOverColor.color;
            currentColor.a = Mathf.Lerp(0f, 1f, t);
            gameOverColor.color = currentColor;
            yield return null;
        }

        gameOverColor.color = new Color(gameOverColor.color.r, gameOverColor.color.g, gameOverColor.color.b, 1f);

        yield return StartCoroutine(TextWriter_co(dialogueMessage));

        yield return StartCoroutine(ShowButtons_co());
    }

    private IEnumerator TextWriter_co(string text)
    {
        gameOverText.text = "";
        foreach (char c in text)
        {
            gameOverText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ShowButtons_co()
    {
        giveup.gameObject.SetActive(true);
        notgiveup.gameObject.SetActive(true);

        Image giveupImg = giveup.GetComponent<Image>();
        Image notgiveupImg = notgiveup.GetComponent<Image>();

        float duration = 1.0f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / duration);

            if (giveupImg) giveupImg.color = SetAlpha(giveupImg.color, alpha);
            if (notgiveupImg) notgiveupImg.color = SetAlpha(notgiveupImg.color, alpha);

            if (giveupText) giveupText.color = SetAlpha(giveupText.color, alpha);
            if (notgiveupText) notgiveupText.color = SetAlpha(notgiveupText.color, alpha);

            yield return null;
        }
    }

    Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}