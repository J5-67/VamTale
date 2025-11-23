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

        // [유니] 전체 슬롯 개수의 절반을 무기 칸으로 쓸게! (예: 6개면 3개)
        int weaponSlotCount = slots.Count / 2;

        // 1. 무기 표시
        for (int i = 0; i < weaponSlotCount; i++)
        {
            if (i < weapons.Length)
                slots[i].SetData(weapons[i].data.itemIcon, weapons[i].level + 1, weapons[i].data.damages.Length);
            else
                slots[i].Hide();
        }

        // 2. 장비 표시 (무기 슬롯 다음부터 시작)
        for (int i = 0; i < weaponSlotCount; i++)
        {
            int slotIndex = i + weaponSlotCount; // 뒷부분 절반은 장비 차지!

            if (i < gears.Length)
                slots[slotIndex].SetData(gears[i].data.itemIcon, gears[i].level + 1, gears[i].data.damages.Length);
            else
                slots[slotIndex].Hide();
        }
    }
}