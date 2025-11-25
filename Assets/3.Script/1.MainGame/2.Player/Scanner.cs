using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;
    public Transform farthestTarget;

    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
        farthestTarget = GetFarthest();
    }

    Transform GetNearest()
    {
        Transform result = null;

        float diff = 100f;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;

            float curDiff = Vector3.Distance(myPos, targetPos);

            if(curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    Transform GetFarthest()
    {
        Transform result = null;
        float diff = 0; // 0부터 시작해서 점점 큰 값을 찾을 거야.

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            // [유니] 현재 거리(curDiff)가 기록된 거리(diff)보다 크면 갱신!
            if (curDiff > diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
}
