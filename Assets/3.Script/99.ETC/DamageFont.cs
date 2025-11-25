using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("튀어오르는 힘")]
    [SerializeField] private float jumpForce = 5f; // 위로 솟구치는 힘
    [SerializeField] private float sideForce = 2f; // 옆으로 퍼지는 힘

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // [유니] 랜덤한 방향으로 튀어 오르게 힘을 줘!
        // 왼쪽(-1) ~ 오른쪽(1) 사이 랜덤
        float randomX = Random.Range(-sideForce, sideForce);

        // 위쪽 방향 + 랜덤 좌우 방향으로 힘 가하기 (Impulse는 순간적인 힘!)
        Vector2 force = new Vector2(randomX, jumpForce);
        rb.AddForce(force, ForceMode2D.Impulse);

        // [유니] 뱅글뱅글 돌면서 떨어지면 더 리얼해!
        rb.AddTorque(Random.Range(-10f, 10f));

        // 2초 뒤에 스스로 사라지기 (청소)
        Destroy(gameObject, 2.0f);
    }
}