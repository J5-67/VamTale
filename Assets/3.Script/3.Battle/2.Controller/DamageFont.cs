using System.Collections;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    [Header("튀어오르는 힘")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float sideForce = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 힘 가하기
        if (rb != null)
        {
            float randomX = Random.Range(-sideForce, sideForce);
            Vector2 force = new Vector2(randomX, jumpForce);
            rb.AddForce(force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-10f, 10f));
        }

        // [유니 수정] 코루틴으로 확실하게 삭제 예약!
        StartCoroutine(Destroy_co());
    }

    IEnumerator Destroy_co()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}