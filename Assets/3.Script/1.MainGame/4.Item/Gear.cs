using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemType type;
    public float rate;

    public ItemData data;
    public int level;

    public void Init(ItemData data)
    {
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        this.data = data;
        this.level = 0;

        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        this.level++;

        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemType.Pie:
                RateUp();
                break;
            case ItemType.Pizza:
                SpeedUp();
                break;
            case ItemType.Spaghetti:
                DamageUp();
                break;
        }
    }

    public void RateUp()
    {
        WeaponController[] weapons = transform.parent.GetComponentsInChildren<WeaponController>();

        foreach (WeaponController weapon in weapons)
        {
            switch (weapon.data.itemType)
            {
                case ItemType.Melee:
                    float speedMelee = 150 + (150 * rate);
                    weapon.speed = Mathf.Min(speedMelee, 200f);
                    break;

                case ItemType.Range:
                    float speedRange = 0.5f * (1f - rate);
                    weapon.speed = Mathf.Max(speedRange, 0.4f);
                    break;

                case ItemType.Lightning:
                    float speedLightning = 0.8f * (1f - rate);
                    weapon.speed = Mathf.Max(speedLightning, 0.25f);
                    break;
            }
        }
    }

    public void SpeedUp()
    {
        float speed = 3;
        GameManager.instance.player.speed = speed + (speed * rate);
    }

    public void DamageUp()
    {
        WeaponController[] weapons = transform.parent.GetComponentsInChildren<WeaponController>();

        foreach (WeaponController weapon in weapons)
        {
            // [유니 수정] 현재 weapon.damage를 쓰지 않고, 데이터 원본에서 계산!
            // 1. 무기의 기본 데미지 가져오기
            float baseDamage = weapon.data.baseDamage;
            float levelBonus = 0;

            // 2. 무기 레벨에 따른 추가 데미지 계산 (Item.cs 로직과 동일하게!)
            // weapon.level은 현재 무기의 레벨이야.
            // 주의: 배열 범위를 넘지 않게 체크!
            if (weapon.level < weapon.data.damages.Length)
            {
                levelBonus = baseDamage * weapon.data.damages[weapon.level];
            }

            // 3. 순수 무기 데미지 (기어 효과 적용 전)
            float originalDamage = baseDamage + levelBonus;

            // 4. 이제 스파게티(기어) 효과 적용! (순수 데미지 + 기어 보너스)
            weapon.damage = originalDamage + (originalDamage * rate);
        }
    }
}