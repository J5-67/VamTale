using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Melee,
    Range,
    Glove,
    Shoe,
    Heal
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("레벨 정보")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("무기")]
    public GameObject projectile;
}
