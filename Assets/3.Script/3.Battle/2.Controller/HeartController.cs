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

        if (minBound != Vector2.zero || maxBound != Vector2.zero)
        {
            nextPos.x = Mathf.Clamp(nextPos.x, minBound.x, maxBound.x);
            nextPos.y = Mathf.Clamp(nextPos.y, minBound.y, maxBound.y);
        }

        rb.MovePosition(nextPos);
    }

    public void SetBoundaries(Bounds bounds)
    {
        float padding = 0.15f;
        minBound = new Vector2(bounds.min.x + padding, bounds.min.y + padding);
        maxBound = new Vector2(bounds.max.x - padding, bounds.max.y - padding);
    }

    public void SetBattleMode(bool isActive)
    {
        isBattleMode = isActive;
        gameObject.SetActive(isActive);

        if (isActive)
        {
            transform.position = new Vector3(0f, 0f, -10f);
        }
    }

    public void OnDamage(float damage)
    {
        if (isInvincible) return;

        GameManager.instance.health -= damage;

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
            OnDamage(10f);
        }
    }
}