using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHUD : MonoBehaviour
{
    public Transform slotRoot;
    public List<ItemSlot> slots;

    private void Awake()
    {
        slots = new List<ItemSlot>();
        for (int i = 0; i < slotRoot.childCount; i++)
        {
            slots.Add(slotRoot.GetChild(i).GetComponent<ItemSlot>());
        }
    }

    private void LateUpdate()
    {
        WeaponController[] weapons = GameManager.instance.player.GetComponentsInChildren<WeaponController>();
        Gear[] gears = GameManager.instance.player.GetComponentsInChildren<Gear>();

        int weaponSlotCount = slots.Count / 2;

        for (int i = 0; i < weaponSlotCount; i++)
        {
            if (i < weapons.Length)
                slots[i].SetData(weapons[i].data.itemIcon, weapons[i].level + 1, weapons[i].data.damages.Length);
            else
                slots[i].Hide();
        }

        for (int i = 0; i < weaponSlotCount; i++)
        {
            int slotIndex = i + weaponSlotCount;

            if (i < gears.Length)
                slots[slotIndex].SetData(gears[i].data.itemIcon, gears[i].level + 1, gears[i].data.damages.Length);
            else
                slots[slotIndex].Hide();
        }
    }
}