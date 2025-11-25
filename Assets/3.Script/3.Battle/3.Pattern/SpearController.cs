using UnityEngine;

public class SpearController : MonoBehaviour
{
    [Header("설정")]
    public float speed = 5f;
    public bool isTrick = false; // 속임수 창인가요?

    [Header("리소스")]
    public Sprite normalSprite;  // 일반 (하늘색)
    public Sprite trickSprite;   // 속임수 (노란색)
    public Sprite warningSprite; // 경고 (빨간색)

    private Transform target;
    private SpriteRenderer sr;
    private bool hasTricked = false;

    public void Init(bool _isTrick, Transform _target)
    {
        isTrick = _isTrick;
        target = _target;
        sr = GetComponent<SpriteRenderer>();

        // 태어날 때 옷 입기
        if (isTrick) sr.sprite = trickSprite;
        else sr.sprite = normalSprite;
    }

    void Update()
    {
        if (target == null) return;

        // 1. 전진
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // 2. 속임수 발동 (거리 2.5 안쪽 + 아직 안 속임)
        if (isTrick && !hasTricked && Vector3.Distance(transform.position, target.position) < 2.5f)
        {
            ExecuteTrick();
        }

        // 3. 플레이어 피격 (거리 0.2 안쪽)
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            Debug.Log("으악! 몸에 맞았다!");
            // 데미지 로직 추가 가능
            Destroy(gameObject);
        }
    }

    void ExecuteTrick()
    {
        hasTricked = true;

        // 위치 반전 (플레이어 기준 대칭)
        transform.position = target.position + (target.position - transform.position);

        // [유니 수정] 속임수 쓸 때도 Z값은 타겟(플레이어)과 똑같이 유지!
        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z);

        // ... (회전 및 속도 로직 그대로) ...
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        speed *= 1.5f;
    }

    public void SetNearest(bool isNearest)
    {
        if (sr == null) return;

        // [유니 수정] 속임수(Trick)라면 절대 빨간색으로 변하지 마! (노란색 유지)
        if (isTrick)
        {
            sr.sprite = trickSprite;
            return;
        }

        // 일반 화살표는 가까우면 빨간색, 아니면 하늘색
        if (isNearest) sr.sprite = warningSprite;
        else sr.sprite = normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // [유니] 충돌 로그 추가! (태그가 맞는지 확인용)
        // Debug.Log($"충돌 감지: {collision.name} / 태그: {collision.tag}");

        if (collision.CompareTag("Shield"))
        {
            // 방패 스크립트가 있으면 효과 실행
            ShieldController shield = collision.GetComponent<ShieldController>();
            if (shield != null) shield.OnBlock();

            Debug.Log("챙! 방어 성공!");
            Destroy(gameObject); // 창 삭제
        }
    }
}