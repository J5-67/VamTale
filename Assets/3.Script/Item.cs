using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public WeaponController weapon;
    public ItemData data;
    public Gear gear;
    public int level;

    private Image icon;
    //private Text textLevel;
    private TMP_Text levelText;
    private TMP_Text nameText;
    private TMP_Text descText;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        //Text[] texts = GetComponentsInChildren<Text>();
        //textLevel = texts[0];

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        levelText = texts[0];
        nameText = texts[1];
        descText = texts[2];
        nameText.text = data.itemName;


    }

    private void OnEnable()
    {
        //textLevel.text = "Lv." + (level + 1);

        levelText.text = string.Format("{0:D2}", level + 1);

        switch (data.itemType)
        {
            case ItemType.Melee:
            case ItemType.Range:
                descText.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemType.Glove:
            case ItemType.Shoe:
                descText.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                descText.text = string.Format(data.itemDesc);
                break;
        }

    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemType.Melee:
            case ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<WeaponController>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemType.Glove:
            case ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;

            case ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }


        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
