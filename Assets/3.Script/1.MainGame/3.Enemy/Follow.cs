using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rt;
    public Vector3 healthBarOffset = new Vector3(-0.07f, 0.7f, 0f);

    private void Awake()
    {
        TryGetComponent<RectTransform>(out rt);
    }

    private void FixedUpdate()
    {
        rt.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position + healthBarOffset);
    }
}
