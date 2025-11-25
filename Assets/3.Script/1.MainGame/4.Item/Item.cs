using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public WeaponController weapon;
    public Gear gear;
    public ItemData data;
    public int level;

    private Image icon;
    private TMP_Text levelText;
    private TMP_Text nameText;
    private TMP_Text descText;

    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        if (images.Length > 1) icon = images[1];
        else icon = images[0];

        if (data != null) icon.sprite = data.itemIcon;

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        if (texts.Length >= 3)
        {
            levelText = texts[0];
            nameText = texts[1];
            descText = texts[2];
            if (data != null) nameText.text = data.itemName;
        }
    }

    private void OnEnable()
    {
        if (data == null || levelText == null || descText == null) return;

        levelText.text = string.Format("{0:D2}", level + 1);

        switch (data.itemType)
        {
            case ItemType.Melee:
            case ItemType.Range:
            case ItemType.Lightning:
                if (level >= data.damages.Length)
                {
                    if (CheckEvolution())
                    {
                        ChangeToEvolveUI();
                        GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        GetComponent<Button>().interactable = false;
                        descText.text = "MAX LEVEL";
                    }
                }
                else
                {
                    descText.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                    GetComponent<Button>().interactable = true;
                }
                break;

            case ItemType.Pie:
            case ItemType.Pizza:
            case ItemType.Spaghetti:
                if (level >= data.damages.Length)
                {
                    GetComponent<Button>().interactable = false;
                    descText.text = "MAX LEVEL";
                }
                else
                {
                    descText.text = string.Format(data.itemDesc, data.damages[level] * 100);
                    GetComponent<Button>().interactable = true;
                }
                break;

            default:
                descText.text = string.Format(data.itemDesc);
                GetComponent<Button>().interactable = true;
                break;
        }
    }

    public bool CheckEvolution()
    {
        if (data.megaWeapon == null)
            return false;

        Gear[] gears = GameManager.instance.player.GetComponentsInChildren<Gear>();
        foreach (Gear g in gears)
        {
            if (g.data == data.needItem && g.level == g.data.damages.Length - 1)
            {
                return true;
            }
        }

        return false;
    }

    void ChangeToEvolveUI()
    {
        if (data.megaWeapon == null) return;

        icon.sprite = data.megaWeapon.itemIcon;
        nameText.text = data.megaWeapon.itemName;
        descText.text = data.megaWeapon.itemDesc;
        levelText.text = "";
    }

    public void OnClick()
    {
        if (data == null) return;

        switch (data.itemType)
        {
            case ItemType.Melee:
            case ItemType.Range:
            case ItemType.Lightning:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<WeaponController>();
                    weapon.Init(data);
                }
                else
                {
                    if (CheckEvolution())
                    {
                        weapon.Evolve(data.megaWeapon);
                    }
                    else
                    {
                        float nextDamage = data.baseDamage;
                        int nextCount = 0;

                        if (level < data.damages.Length)
                        {
                            nextDamage += data.baseDamage * data.damages[level];
                            nextCount += data.counts[level];
                        }

                        weapon.LevelUp(nextDamage, nextCount);
                    }
                }
                level++;
                break;

            case ItemType.Pie:
            case ItemType.Pizza:
            case ItemType.Spaghetti:
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