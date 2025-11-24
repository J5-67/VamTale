using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartController : MonoBehaviour
{
    [Header("설정")]
    public float moveSpeed = 5f;
    public bool isBattleMode = false;

    // [유니] 이제 인스펙터에서 안 정하고, 코드로 주입받을 거야!
    private Vector2 minBound;
    private Vector2 maxBound;

    // 무적 관련
    public float invincibilityTime = 1.0f;
    private bool isInvincible = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (rb != null) rb.gravityScale = 0;
    }

    void Update()
    {
        if (!isBattleMode) return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v).normalized;
    }

    void FixedUpdate()
    {
        if (!isBattleMode || rb == null) return;

        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        // [유니] 설정된 범위(Bound) 안으로 가두기!
        // (값이 0이면 제한 없다고 칠 수도 있지만, 배틀에선 무조건 제한이 있어야 해)
        if (minBound != Vector2.zero || maxBound != Vector2.zero)
        {
            nextPos.x = Mathf.Clamp(nextPos.x, minBound.x, maxBound.x);
            nextPos.y = Mathf.Clamp(nextPos.y, minBound.y, maxBound.y);
        }

        rb.MovePosition(nextPos);
    }

    // [유니 핵심] 외부(BattleManager)에서 이동 범위를 정해주는 함수!
    public void SetBoundaries(Bounds bounds)
    {
        // Bounds.min/max는 월드 좌표 기준이라 아주 정확해!
        // 하트 크기(반지름)만큼 살짝 빼주면 더 완벽하게 안쪽에 갇혀. (여기선 0.15f 정도 여유 둠)
        float padding = 0.15f;
        minBound = new Vector2(bounds.min.x + padding, bounds.min.y + padding);
        maxBound = new Vector2(bounds.max.x - padding, bounds.max.y - padding);
    }

    public void SetBattleMode(bool isActive)
    {
        isBattleMode = isActive;
        gameObject.SetActive(isActive);
        if (isActive) transform.localPosition = Vector3.zero;
    }

    public void OnDamage(float damage)
    {
        if (isInvincible) return;

        GameManager.instance.health -= damage;
        Debug.Log($"으악! 남은 체력: {GameManager.instance.health}");

        if (GameManager.instance.health <= 0)
        {
            GameManager.instance.health = 0;
            SceneManager.LoadScene("GameOver");
            return;
        }

        StartCoroutine(Invincibility_co());
    }

    System.Collections.IEnumerator Invincibility_co()
    {
        isInvincible = true;
        for (int i = 0; i < 5; i++)
        {
            sr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Spear"))
        {
            OnDamage(10f); // 데미지 수치는 나중에 데이터에서 가져오면 더 좋아!
        }
    }
}