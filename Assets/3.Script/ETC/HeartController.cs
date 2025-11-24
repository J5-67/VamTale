using UnityEngine;

public class HeartController : MonoBehaviour
{
    [Header("설정")]
    public float moveSpeed = 5f;
    public bool isBattleMode = false; // 전투 중인가요?

    [Header("제한 구역 (BattleBox 안에서만 움직이게)")]
    public Vector2 minBound; // 좌측 하단 (예: -2, -2)
    public Vector2 maxBound; // 우측 상단 (예: 2, 2)

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // [유니] 배틀 매니저 연결 (나중에 필요할 수 있음)
    private BattleManager battleManager;

    public void SetBattleManager(BattleManager manager)
    {
        battleManager = manager;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 하트는 중력 받으면 안 됨!
        if (rb != null) rb.gravityScale = 0;
    }

    void Update()
    {
        // 배틀 모드가 아니거나 꺼져있으면 움직이지 마
        if (!isBattleMode) return;

        // 입력 받기 (Legacy Input)
        float h = 0;
        float v = 0;

        if (Input.GetKey(KeyCode.UpArrow)) v = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) v = -1;

        if (Input.GetKey(KeyCode.RightArrow)) h = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) h = -1;

        moveInput = new Vector2(h, v).normalized;
    }

    void FixedUpdate()
    {
        if (!isBattleMode || rb == null) return;

        // 이동
        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        // [유니 꿀팁] 화면 밖으로 나가지 않게 가두기! (Clamp)
        // (BattleBox의 크기에 맞춰서 minBound, maxBound를 인스펙터에서 설정해줘!)
        nextPos.x = Mathf.Clamp(nextPos.x, minBound.x, maxBound.x);
        nextPos.y = Mathf.Clamp(nextPos.y, minBound.y, maxBound.y);

        rb.MovePosition(nextPos);
    }

    // [유니] 전투 시작할 때 호출해줘!
    public void SetBattleMode(bool isActive)
    {
        isBattleMode = isActive;
        // 켜질 때 위치 초기화 (중앙)
        if (isActive) transform.localPosition = Vector3.zero;
    }
}