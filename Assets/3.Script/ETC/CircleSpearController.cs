using System.Collections;
using UnityEngine;

public class CircleSpearController : MonoBehaviour
{
    [Header("설정")]
    public float speed = 5f;     // 돌진 속도
    public float rushDelay = 1.0f; // 뜸 들이는 시간

    [Header("리소스")]
    public Sprite normalSprite;  // 평소 색 (하늘색 등)

    private SpriteRenderer sr;
    private bool isRushing = false; // "나 지금 달리고 있니?"

    // [유니] 태어날 때(Init) 필요한 정보만 쏙쏙!
    public void Init(float _rushDelay)
    {
        rushDelay = _rushDelay;
        sr = GetComponent<SpriteRenderer>();

        if (sr != null) sr.sprite = normalSprite; // 처음엔 평범하게

        // 코루틴 시작: 대기 -> 깜빡 -> 돌진!
        StartCoroutine(CircleRush_co());
    }

    void Update()
    {
        // 돌진 신호가 떨어지면 앞만 보고 달려!
        if (isRushing)
        {
            // transform.up은 "화살표 머리 방향"이야. (이미지 회전에 따라 right일 수도 있음)
            // 오빠 화살표가 "오른쪽"을 본다면 transform.right를 써야 할 수도 있어!
            // 일단 회전을 잘 시켰다고 가정하고 up으로 갈게. (이상하면 말해줘!)
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        // 화면 밖으로 나가면 삭제 (청소)
        if (Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator CircleRush_co()
    {
        // 1. 뜸 들이기 (플레이어 심리 압박)
        yield return new WaitForSeconds(rushDelay * 1f);

        // 3. 돌진 시작!
        isRushing = true;
        speed *= 3.0f; // 속도 확 올려서 피하기 어렵게!
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어(빨간 하트)에 닿으면 데미지!
        if (collision.CompareTag("Player"))
        {
            Debug.Log("으악! 원형 창에 찔렸다!");
            // 데미지 처리 로직 (GameManager.Instance.player.OnDamage...)
            Destroy(gameObject);
        }
    }
}