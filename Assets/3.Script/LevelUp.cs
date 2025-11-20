using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public RectTransform rect;
    Item[] items;

    private void Awake()
    {
        TryGetComponent<RectTransform>(out rect);
        items = GetComponentsInChildren<Item>(true);
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

        int[] rand = new int[3];
        while (true)
        {
            rand[0] = UnityEngine.Random.Range(0, items.Length);
            rand[1] = UnityEngine.Random.Range(0, items.Length);
            rand[2] = UnityEngine.Random.Range(0, items.Length);

            if (rand[0] != rand[1] && rand[1] != rand[2] && rand[0] != rand[2])
            {
                break;
            }
        }

        for (int i = 0; i < rand.Length; i++)
        {
            Item randItem = items[rand[i]];


            if (randItem.level == randItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true);
            }
            else
            {
                randItem.gameObject.SetActive(true);
            }

        }
    }
}
