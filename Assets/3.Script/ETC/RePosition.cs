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

        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.input;
        float dirX = playerDir.x < 0 ? 1 : -1;
        float dirY = playerDir.y < 0 ? 1 : -1;

        switch (transform.tag)
        {
            case "Ground":
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
                    transform.Translate(playerDir * 20 + new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0f));
                }
                break;
        }
    }
}
