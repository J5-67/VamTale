using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum InfoType
{
    Exp,
    Level,
    Kill,
    Time,
    Health
}

public class HUD : MonoBehaviour
{
    public InfoType type;

    private TMP_Text myText;
    private Slider mySlider;

    private void Awake()
    {
        TryGetComponent<TMP_Text>(out myText);
        TryGetComponent<Slider>(out mySlider);
    }

    private void LateUpdate()
    {
        if(GameManager.instance == null || GameManager.instance.player == null)
        {
            Debug.Log("HUD instance error");
            return;
        }

        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                //myText.text = $"LV. {GameManager.instance.level}";
                myText.text = string.Format("LV.{0:f0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:f0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }

}
