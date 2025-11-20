using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemType.Glove:
                RateUp();
                    break;
            case ItemType.Shoe:
                SpeedUp();
                    break;
        }
    }

    public void RateUp()
    {
        WeaponController[] weapons = transform.parent.GetComponentsInChildren<WeaponController>();

        foreach(WeaponController weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    weapon.speed = 150 + (150 * rate);
                    break;
                default:
                    weapon.speed = 0.5f * (1f - rate);
                    break;
            }
        }
    }

    public void SpeedUp()
    {
        float speed = 5;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
