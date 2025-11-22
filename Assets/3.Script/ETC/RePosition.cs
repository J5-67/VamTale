using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePosition : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private float mapSizeX = 38f;
    [SerializeField] private float mapSizeY = 38f;
    private Collider2D coll;

    private void Awake()
    {
        TryGetComponent<Collider2D>(out coll);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || GameManager.instance == null || GameManager.instance.player == null)
        {
            return;
        }

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        switch (transform.tag)
        {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;

                float dirX = diffX < 0 ? 1 : -1;
                float dirY = diffY < 0 ? 1 : -1;

                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * mapSizeX);
                }
                if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * mapSizeY);
                
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    Vector3 rand = new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), 0);

                    Vector3 dist = playerPos - myPos;
                    
                    transform.Translate(rand + dist * 2);
                }
                break;
        }
    }
}
