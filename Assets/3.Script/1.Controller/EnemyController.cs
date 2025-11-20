using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] anCon;
    private bool isLive = true;
    private bool isKnockBack = false;

    private Rigidbody2D playerRg;
    private Collider2D co;
    private Rigidbody2D rg;
    private Animator an;
    private SpriteRenderer sp;

    WaitForFixedUpdate wait;

    private void Awake()
    {
        TryGetComponent<Rigidbody2D>(out rg);
        TryGetComponent<Animator>(out an);
        TryGetComponent<SpriteRenderer>(out sp);
        TryGetComponent<Collider2D>(out co);
        wait = new WaitForFixedUpdate();

        if(GameManager.instance != null && GameManager.instance.player != null)
        {
            playerRg = GameManager.instance.player.GetComponent<Rigidbody2D>();
        }
    }
    private void OnEnable()
    {
        isLive = true;
        isKnockBack = false;
        health = maxHealth;
        //co.enabled = true;
        //rg.simulated = true;
        //sp.sortingOrder = 2;
        //죽는 애니메이션 an.SetBol("Dead", false);
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (!isLive || playerRg == null || isKnockBack)
        {
            return;
        }

        Vector2 dirVec = playerRg.position - rg.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rg.MovePosition(rg.position + nextVec);
        rg.linearVelocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (!isLive || playerRg == null)
        {
            return;
        }

        sp.flipX = playerRg.position.x > rg.position.x;
    }


    public void Init(SpawnData spawnData)
    {
        an.runtimeAnimatorController = anCon[spawnData.spriteType];
        speed = spawnData.speed;
        maxHealth = spawnData.health;
        health = spawnData.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
        {
            return;
        }

        health -= collision.GetComponent<BulletController>().damage;
        StartCoroutine(KnockBack_co());

        if (health > 0)
        {

        }
        else
        {
            isLive = false;
            //co.enabled = false;
            //rg.simulated = false;
            //sp.sortingOrder = 1;
            //죽는 애니메이션 an.SetBol("Dead", true);
            Dead(); //9강 17분 애니메이션으로 실행하기 보기.
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }

    public IEnumerator KnockBack_co()
    {
        sp.color = Color.white;
        isKnockBack = true;

        yield return wait;

        sp.color = Color.red;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rg.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);

        sp.color = Color.white;
        isKnockBack = false;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }
}
