using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] public GameObject[] prefabs;
    public List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int i= 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int i)
    {
        GameObject select = null;

        foreach(GameObject enemy in pools[i])
        {
            if (!enemy.activeSelf)
            {
                select = enemy;
                select.SetActive(true);
                break;
            }
        }

        if(select == null)
        {
            select = Instantiate(prefabs[i], transform);
            pools[i].Add(select);
        }



        return select;
    }
}
