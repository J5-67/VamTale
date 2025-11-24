using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState
{
    Dialogue,   // 대화 상태
    Attack,     // 공격 타이밍 맞추기
    EnemyTurn,  // 적 턴 (패턴 회피)
    Win,        // 승리
    Lose        // 패배
}

public class BattleManager : MonoBehaviour
{
    [Header("--- 핵심 연결 ---")]
    [SerializeField] private EnemyData targetEnemy;
    [SerializeField] private PatternManager patternManager;

    [Header("--- UI 오브젝트 ---")]
    [SerializeField] private GameObject playerRedHeart;
    [SerializeField] private GameObject playerGreenHeart;
    [SerializeField] private GameObject dialogueBox;

    // [유니 수정] 배틀 박스를 3개로 관리! (0: 작음, 1: 중간, 2: 큼 등)
    [SerializeField] private GameObject[] battleBoxes;

    [SerializeField] private TMP_Text dialogueText;

    [Header("--- 공격 UI ---")]
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private Transform attackCursor;
    [SerializeField] private float cursorSpeed = 500f;
    private bool isCursorMovingRight = true;
    private float attackWidth = 500f;

    [Header("--- 데미지 연출 ---")]
    // [유니 추가] 낮은 데미지(빗맞음) -> 높은 데미지(정중앙) 순서로 넣어줘!
    [SerializeField] private GameObject[] damageNumberPrefabs;

    private BattleState state;
    private int currentHP;
    private int currentBoxIndex = 0; // 현재 활성화된 배틀 박스 번호

    void Start()
    {
        if (targetEnemy != null)
        {
            targetEnemy.CalEnemyHP();
            currentHP = targetEnemy.CurrentHP;
        }

        // 초기화
        attackPanel.SetActive(false);
        CloseAllBattleBoxes();
        ChangeState(BattleState.Dialogue);
    }

    void Update()
    {
        switch (state)
        {
            case BattleState.Dialogue:
                if (Input.GetKeyDown(KeyCode.Z)) ChangeState(BattleState.Attack);
                break;

            case BattleState.Attack:
                MoveAttackCursor();
                if (Input.GetKeyDown(KeyCode.Z)) ExecuteAttack();
                break;
        }
    }

    void ChangeState(BattleState newState)
    {
        state = newState;

        // UI 상태 정리
        dialogueBox.SetActive(false);
        CloseAllBattleBoxes(); // 배틀 박스 끄기
        attackPanel.SetActive(false);
        playerRedHeart.SetActive(false);

        switch (state)
        {
            case BattleState.Dialogue:
                dialogueBox.SetActive(true);
                playerRedHeart.SetActive(true);
                // 대화 상태일 때 하트 위치 (예: 대화창 왼쪽)
                playerRedHeart.transform.localPosition = new Vector3(-300, -200, 0);
                if (dialogueText != null) dialogueText.text = $"야생의 {targetEnemy.EnemyName}이(가) 승부를 걸어왔다!";
                break;

            case BattleState.Attack:
                attackPanel.SetActive(true);
                if (attackCursor != null) attackCursor.localPosition = new Vector3(-attackWidth / 2, 0, 0);
                break;

            case BattleState.EnemyTurn:
                // [유니] 적 턴 시작할 때 정해진 박스 켜기
                if (battleBoxes != null && battleBoxes.Length > currentBoxIndex)
                {
                    battleBoxes[currentBoxIndex].SetActive(true);
                }

                playerRedHeart.SetActive(true);
                playerRedHeart.transform.position = Vector3.zero; // 박스 중앙으로 이동
                StartCoroutine(EnemyPattern_co());
                break;
        }
    }

    void MoveAttackCursor()
    {
        if (attackCursor == null) return;
        float move = cursorSpeed * Time.deltaTime;

        if (isCursorMovingRight)
        {
            attackCursor.localPosition += Vector3.right * move;
            if (attackCursor.localPosition.x >= attackWidth / 2) isCursorMovingRight = false;
        }
        else
        {
            attackCursor.localPosition -= Vector3.right * move;
            if (attackCursor.localPosition.x <= -attackWidth / 2) isCursorMovingRight = true;
        }
    }

    // [유니 핵심 수정] 공격 실행 로직
    void ExecuteAttack()
    {
        // 1. 정확도 계산 (1.0 = 정중앙, 0.0 = 끝)
        float distance = Mathf.Abs(attackCursor.localPosition.x);
        float accuracy = 1f - (distance / (attackWidth / 2));
        accuracy = Mathf.Clamp(accuracy, 0f, 1f);

        // 2. 실질적 데미지는 무조건 1 (오빠 요청!)
        int realDamage = 1;

        // (선택) 너무 빗나갔으면(정확도 10% 미만) MISS 처리 할까?
        if (accuracy < 0.1f) realDamage = 0;

        if (realDamage > 0)
        {
            currentHP -= realDamage;
            // 3. 정확도에 따른 비주얼 데미지 숫자 띄우기! 
            SpawnDamageNumber(accuracy);
        }
        else
        {
            Debug.Log("MISS! 빗나갔다...");
        }

        // 4. 죽음 체크
        if (currentHP <= 0)
        {
            ChangeState(BattleState.Win);
            BattleEnd();
        }
        else
        {
            ChangeState(BattleState.EnemyTurn);
        }
    }

    // [유니 추가] 데미지 숫자 이미지 생성 함수
    void SpawnDamageNumber(float accuracy)
    {
        if (damageNumberPrefabs == null || damageNumberPrefabs.Length == 0) return;

        // 정확도(0~1)를 배열 인덱스로 변환
        // 예: 이미지가 3개라면 [0:나쁨, 1:보통, 2:완벽]
        int index = Mathf.FloorToInt(accuracy * damageNumberPrefabs.Length);
        index = Mathf.Clamp(index, 0, damageNumberPrefabs.Length - 1);

        GameObject prefab = damageNumberPrefabs[index];

        // 적 위치(또는 화면 중앙)에 생성
        Vector3 spawnPos = new Vector3(0, 2.5f, 0);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    IEnumerator EnemyPattern_co()
    {
        yield return new WaitForSeconds(0.5f);

        // [유니] 패턴에 따라 박스 크기 정하기 (랜덤 예시)
        // 나중엔 PatternManager가 패턴이랑 박스 인덱스를 같이 주면 좋아!
        currentBoxIndex = Random.Range(0, battleBoxes.Length);
        ChangeState(BattleState.EnemyTurn); // 박스 갱신을 위해 상태 재호출 (혹은 여기서 SetActive)

        if (patternManager != null)
        {
            patternManager.StartPattern(targetEnemy, Vector3.zero);
        }

        yield return new WaitForSeconds(3.0f); // 패턴 지속 시간

        if (patternManager != null) patternManager.StopMonsterAttack();

        ChangeState(BattleState.Dialogue);
    }

    void CloseAllBattleBoxes()
    {
        if (battleBoxes == null) return;
        foreach (var box in battleBoxes)
        {
            if (box != null) box.SetActive(false);
        }
    }

    void BattleEnd()
    {
        Debug.Log("전투 승리!");
        // SceneManager.LoadScene("MainGame");
    }
}