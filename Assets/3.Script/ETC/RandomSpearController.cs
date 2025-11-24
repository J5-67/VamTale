using System.Collections;
using UnityEngine;

public class RandomSpearController : MonoBehaviour
{
    [Header("설정")]
    public float speed = 7f;      // 돌진 속도
    public float rushDelay = 1.0f; // 조준 대기 시간

    [Header("리소스")]
    public Sprite normalSprite;   // 평소 색

    private SpriteRenderer sr;
    private Transform target;
    private bool isRushing = false;

    public void Init(Transform _target, float _rushDelay)
    {
        target = _target;
        rushDelay = _rushDelay;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) sr.sprite = normalSprite;

        if (target != null)
        {
            Vector3 dir = target.position - transform.position;

            // 아크탄젠트로 각도 구하기 (이건 오른쪽 기준 각도야)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // [유니 수정] 오빠 그림이 "위쪽"을 보고 있으니까 90도를 빼줘야 정면이 돼!
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        StartCoroutine(Rush_co());
    }

    void Update()
    {
        if (isRushing)
        {
            // [유니 수정] 그림의 머리 방향(위쪽)으로 직진!
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        // 화면 밖으로 나가면 삭제 (청소)
        if (Mathf.Abs(transform.position.x) > 12f || Mathf.Abs(transform.position.y) > 12f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Rush_co()
    {
        // 1. 대기 (플레이어를 겨누고 멈춰있음)
        yield return new WaitForSeconds(rushDelay * 0.7f);

        // 3. 돌진 시작!
        isRushing = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("으악! 랜덤 창에 찔렸다!");
            // 데미지 처리 로직
            Destroy(gameObject);
        }
    }
}