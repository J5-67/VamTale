using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState
{
    Dialogue,       // 1. 내 턴 (메뉴 선택 / FIGHT 버튼 위)
    EnemySelect,    // 2. 적 선택
    Attack,         // 3. 공격 게이지
    EnemyTurn,      // 4. 적 턴 (방어/회피)
    Win,
    Lose
}

public class BattleManager : MonoBehaviour
{
    [Header("--- 핵심 연결 ---")]
    [SerializeField] private EnemyData targetEnemy;
    [SerializeField] private PatternManager patternManager;

    [Header("--- 하트 4총사 ---")]
    [SerializeField] private GameObject menuRedHeart;    // 메뉴용 (빨강)
    [SerializeField] private GameObject menuGreenHeart;  // 메뉴용 (초록)
    [SerializeField] private GameObject combatRedHeart;  // 전투용 (빨강)
    [SerializeField] private GameObject combatGreenHeart;// 전투용 (초록)

    [Header("--- 하트 위치 (빈 오브젝트 연결) ---")]
    private Vector3 posOnButton = new Vector3(-1.05f, -4.2f, -5f);
    private Vector3 posOnSelect = new Vector3(-5.13f, -0.10f, -5f); 

    [Header("--- 배틀 박스 (Collider 필수) ---")]
    // 0: 방패 패턴용(작음), 1: 원형 패턴용(중간), 2: 랜덤 패턴용(큼)
    [SerializeField] private GameObject[] battleBoxes;

    [Header("--- UI 오브젝트 ---")]
    [SerializeField] private GameObject dialogueBox;     // 흰색 배경 (DialogueWhite)
    [SerializeField] private GameObject enemySelectPanel;// 적 선택 패널
    [SerializeField] private Slider enemyHPSlider;
    [SerializeField] private TMP_Text dialogueText;      // 대화 텍스트

    [Header("--- 공격 버튼 스프라이트 (수정됨!) ---")]
    // [유니 수정] 오빠 말대로 SpriteRenderer로 변경했어!
    [SerializeField] private SpriteRenderer fightButtonRenderer;
    [SerializeField] private Sprite fightNormalSprite;   // 평소 이미지 (주황색/흰색)
    [SerializeField] private Sprite fightHighlightSprite;// 하트 올라갔을 때 이미지 (초록색 등)

    [Header("--- 공격 UI ---")]
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private Transform attackCursor;
    [SerializeField] private float cursorSpeed = 500f;
    [SerializeField] private GameObject[] damageNumberPrefabs;
    private bool isCursorMovingRight = true;
    private float attackWidth = 500f;

    // 내부 변수
    private BattleState state;
    private int currentHP;
    public bool isGreenMode = true; // 시작은 초록 하트

    void Start()
    {
        if (targetEnemy != null)
        {
            targetEnemy.CalEnemyHP();
            currentHP = targetEnemy.CurrentHP;
            if (enemyHPSlider != null)
            {
                enemyHPSlider.maxValue = targetEnemy.MaxHP;
                enemyHPSlider.value = currentHP;
            }
        }

        // 초기화
        DisableAllHearts();
        CloseAllBattleBoxes();
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        // 시작 상태 설정
        isGreenMode = true;
        menuGreenHeart.transform.position = posOnButton;
        ChangeState(BattleState.Dialogue);
    }

    void Update()
    {
        switch (state)
        {
            case BattleState.Dialogue:
                // Z키 누르면 적 선택 화면으로 이동
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    ChangeState(BattleState.EnemySelect);
                }
                break;

            case BattleState.EnemySelect:
                // Z키 누르면 공격 시작
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    ChangeState(BattleState.Attack);
                }
                // X키 누르면 다시 메뉴로 돌아가기
                if (Input.GetKeyDown(KeyCode.X))
                {
                    ChangeState(BattleState.Dialogue);
                }
                break;

            case BattleState.Attack:
                MoveAttackCursor();
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    ExecuteAttack();
                }
                break;
        }
    }

    void ChangeState(BattleState newState)
    {
        state = newState;

        // UI 상태 초기화
        DisableAllHearts();
        CloseAllBattleBoxes();
        attackPanel.SetActive(false);

        // [수정] 버튼 스프라이트 초기화 (평소 상태로)
        if (fightButtonRenderer != null && fightNormalSprite != null)
            fightButtonRenderer.sprite = fightNormalSprite;

        switch (state)
        {
            case BattleState.Dialogue:
                // DialogueBox(배경) 켜기, 텍스트 켜기, 적 선택창 끄기
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(true);
                enemySelectPanel.SetActive(false);

                // 메뉴용 하트 켜기 (버튼 위 위치)
                ActivateMenuHeart(posOnButton);

                // [수정] 버튼 하이라이트 켜기 (SpriteRenderer)
                if (fightButtonRenderer != null && fightHighlightSprite != null)
                    fightButtonRenderer.sprite = fightHighlightSprite;

                if (dialogueText != null)
                    dialogueText.text = $"* 영웅이 나타났다";
                break;

            case BattleState.EnemySelect:
                // DialogueBox(배경) 유지, 텍스트만 끄기
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);

                enemySelectPanel.SetActive(true);

                // 메뉴용 하트 켜기 (적 선택 위치)
                ActivateMenuHeart(posOnSelect);
                break;

            case BattleState.Attack:
                // 공격 중에도 배경은 켜두기
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                enemySelectPanel.SetActive(false);

                attackPanel.SetActive(true);
                if (attackCursor != null)
                    attackCursor.localPosition = new Vector3(-attackWidth / 2, 0, 0);
                break;

            case BattleState.EnemyTurn:
                dialogueBox.SetActive(false); // 적 턴엔 대화창 끄기
                enemySelectPanel.SetActive(false);

                StartCoroutine(EnemyPattern_co());
                break;
        }
    }

    // [유니] 메뉴 하트 활성화
    void ActivateMenuHeart(Vector3 localPos)
    {
        GameObject heartToUse = isGreenMode ? menuGreenHeart : menuRedHeart;
        if (heartToUse != null)
        {
            heartToUse.SetActive(true);
            heartToUse.transform.localPosition = localPos;
        }
    }

    IEnumerator EnemyPattern_co()
    {
        // 1. 패턴 및 박스 결정
        int boxIndex = 0;
        bool useGreenCombat = false;

        if (isGreenMode)
        {
            boxIndex = 0; // 방패 패턴용 작은 박스
            useGreenCombat = true;
        }
        else
        {
            int rnd = Random.Range(0, 2);
            boxIndex = (rnd == 0) ? 1 : 2; // 1:원형(중간), 2:랜덤(큰)
            useGreenCombat = false;
        }

        // 2. 박스 켜기 및 이동 제한 설정
        if (battleBoxes != null && battleBoxes.Length > boxIndex)
        {
            GameObject activeBox = battleBoxes[boxIndex];
            activeBox.SetActive(true);

            // [중요] 빨간 하트 이동 범위 설정
            BoxCollider2D boxCol = activeBox.GetComponent<BoxCollider2D>();
            if (boxCol != null && !useGreenCombat)
            {
                combatRedHeart.GetComponent<HeartController>().SetBoundaries(boxCol.bounds);
            }
        }

        // 3. 전투용 하트 켜기
        GameObject combatHeart = useGreenCombat ? combatGreenHeart : combatRedHeart;
        if (combatHeart != null)
        {
            combatHeart.SetActive(true);
            combatHeart.transform.localPosition = Vector3.zero;

            // 빨간 하트라면 이동 스크립트 활성화
            if (!useGreenCombat)
                combatHeart.GetComponent<HeartController>().SetBattleMode(true);
        }

        yield return new WaitForSeconds(0.5f);

        // 4. 패턴 실행
        if (patternManager != null)
        {
            if (isGreenMode) patternManager.StartShieldPattern(); // 4방향 방패 (함수 이름 확인!)
            else
            {
                if (boxIndex == 1) patternManager.StartCirclePattern(); // 원형
                else patternManager.StartRandomPattern(); // 랜덤 돌진 (함수 이름 확인!)
            }
        }

        yield return new WaitForSeconds(5.0f); // 패턴 지속 시간

        if (patternManager != null) patternManager.StopMonsterAttack();

        // 이동 멈춤
        if (!useGreenCombat && combatRedHeart != null)
            combatRedHeart.GetComponent<HeartController>().SetBattleMode(false);

        // 턴 종료 후 모드 전환
        isGreenMode = !isGreenMode;

        ChangeState(BattleState.Dialogue);
    }

    // --- 기본 기능들 ---
    void DisableAllHearts()
    {
        if (menuRedHeart) menuRedHeart.SetActive(false);
        if (menuGreenHeart) menuGreenHeart.SetActive(false);
        if (combatRedHeart) combatRedHeart.SetActive(false);
        if (combatGreenHeart) combatGreenHeart.SetActive(false);
    }

    void CloseAllBattleBoxes()
    {
        if (battleBoxes == null) return;
        foreach (var box in battleBoxes) if (box != null) box.SetActive(false);
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

    void ExecuteAttack()
    {
        float distance = Mathf.Abs(attackCursor.localPosition.x);
        float accuracy = 1f - (distance / (attackWidth / 2));
        accuracy = Mathf.Clamp(accuracy, 0f, 1f);

        int realDamage = 1;
        if (accuracy < 0.1f) realDamage = 0;

        if (realDamage > 0)
        {
            currentHP -= realDamage;
            if (enemyHPSlider != null) enemyHPSlider.value = currentHP;
            SpawnDamageNumber(accuracy);
        }
        else
        {
            Debug.Log("MISS!");
        }

        if (currentHP <= 0)
        {
            ChangeState(BattleState.Win);
            Debug.Log("승리!");
        }
        else
        {
            ChangeState(BattleState.EnemyTurn);
        }
    }

    void SpawnDamageNumber(float accuracy)
    {
        if (damageNumberPrefabs == null || damageNumberPrefabs.Length == 0) return;
        int index = Mathf.FloorToInt(accuracy * damageNumberPrefabs.Length);
        index = Mathf.Clamp(index, 0, damageNumberPrefabs.Length - 1);
        Instantiate(damageNumberPrefabs[index], new Vector3(0, 2.5f, 0), Quaternion.identity);
    }
}