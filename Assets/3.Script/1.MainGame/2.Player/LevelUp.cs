using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public RectTransform rect;
    Item[] items;
    Item healItem;

    private void Awake()
    {
        TryGetComponent(out rect);
        items = GetComponentsInChildren<Item>(true);

        foreach (Item item in items)
        {
            if (item.data != null && item.data.itemType == ItemType.Heal)
            {
                healItem = item;
                break;
            }
        }
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    public void Next()
    {
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        List<Item> activeItems = new List<Item>();
        int attempts = 0;

        while (activeItems.Count < 3 && attempts < 100)
        {
            attempts++;
            int index = UnityEngine.Random.Range(0, items.Length);
            Item randomItem = items[index];

            if (randomItem.data == null)
            {
                continue;
            }

            Item finalItemToShow = randomItem;

            // [유니] 만렙(Max Level) 아이템 처리 로직
            if (randomItem.level >= randomItem.data.damages.Length)
            {
                // 1. 이미 진화된 무기를 가지고 있는지 확인!
                bool alreadyEvolved = false;

                // 플레이어의 무기들을 다 뒤져서 확인
                foreach (WeaponController w in GameManager.instance.player.GetComponentsInChildren<WeaponController>())
                {
                    // 만약 내 '진화 결과물(MegaWeapon)'을 이미 플레이어가 들고 있다면?
                    if (w.data == randomItem.data.megaWeapon)
                    {
                        alreadyEvolved = true;
                        break;
                    }
                }

                // 2. 상황에 따른 처리
                if (alreadyEvolved)
                {
                    // 이미 진화했으면 이 버튼은 필요 없음 -> 회복약으로 교체
                    if (healItem != null) finalItemToShow = healItem;
                }
                else
                {
                    // 아직 진화 안 했음! 진화 조건(CheckEvolution) 확인
                    if (randomItem.CheckEvolution())
                    {
                        // 진화 가능! 그대로 둠 (진화 버튼 등판!)
                    }
                    else
                    {
                        // 진화 조건 안 맞음 -> 회복약으로 교체
                        if (healItem != null) finalItemToShow = healItem;
                    }
                }
            }

            // 중복 검사
            if (activeItems.Contains(finalItemToShow))
            {
                continue;
            }

            activeItems.Add(finalItemToShow);
            finalItemToShow.gameObject.SetActive(true);
        }
    }
}