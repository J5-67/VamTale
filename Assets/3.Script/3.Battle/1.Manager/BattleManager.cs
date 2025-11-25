using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState
{
    Dialogue,
    EnemySelect,
    Attack,
    EnemyTurn,
    Win,
    Lose
}

public class BattleManager : MonoBehaviour
{
    [Header("--- 핵심 연결 ---")]
    [SerializeField] private EnemyData targetEnemy;
    [SerializeField] private PatternManager patternManager;

    [Header("--- 하트 4총사 ---")]
    [SerializeField] private GameObject menuRedHeart;
    [SerializeField] private GameObject menuGreenHeart;
    [SerializeField] private GameObject combatRedHeart;
    [SerializeField] private GameObject combatGreenHeart;

    [Header("--- 하트 위치 ---")]
    private Vector3 posOnButton = new Vector3(-1.05f, -4.2f, -5f);
    private Vector3 posOnSelect = new Vector3(-5.13f, -0.10f, -5f);

    [Header("--- 배틀 박스 ---")]
    [SerializeField] private GameObject[] battleBoxes;

    [Header("--- UI 오브젝트 ---")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject enemySelectPanel;
    [SerializeField] private Slider enemyHPSlider;
    [SerializeField] private TMP_Text dialogueText;

    [Header("--- 공격 버튼 스프라이트 ---")]
    [SerializeField] private SpriteRenderer fightButtonRenderer;
    [SerializeField] private Sprite fightNormalSprite;
    [SerializeField] private Sprite fightHighlightSprite;

    [Header("--- 공격 UI ---")]
    [SerializeField] private GameObject attackPanel;
    [SerializeField] private Transform attackCursor;
    [SerializeField] private float cursorSpeed = 500f;
    [SerializeField] private GameObject[] damageNumberPrefabs;

    [SerializeField] private BoxCollider2D dialogueBlackCollider;
    [SerializeField] private float widthMultiplier = 11.8f;

    private bool isCursorMovingRight = true;
    private float attackWidth;

    private BattleState state;
    private int currentHP;
    public bool isGreenMode = true;

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

        if (dialogueBlackCollider != null)
        {
            attackWidth = dialogueBlackCollider.size.x * widthMultiplier;
        }
        else
        {
            if (attackPanel != null && attackPanel.TryGetComponent(out RectTransform rect))
                attackWidth = rect.rect.width - 20f;
            else
                attackWidth = 500f;
        }

        DisableAllHearts();
        CloseAllBattleBoxes();
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);

        isGreenMode = true;
        ChangeState(BattleState.Dialogue);
    }

    void Update()
    {
        switch (state)
        {
            case BattleState.Dialogue:
                if (Input.GetKeyDown(KeyCode.Z)) ChangeState(BattleState.EnemySelect);
                break;

            case BattleState.EnemySelect:
                if (Input.GetKeyDown(KeyCode.Z)) ChangeState(BattleState.Attack);
                if (Input.GetKeyDown(KeyCode.X)) ChangeState(BattleState.Dialogue);
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

        DisableAllHearts();
        CloseAllBattleBoxes();
        attackPanel.SetActive(false);

        if (fightButtonRenderer != null && fightNormalSprite != null)
            fightButtonRenderer.sprite = fightNormalSprite;

        switch (state)
        {
            case BattleState.Dialogue:
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(true);
                enemySelectPanel.SetActive(false);

                ActivateMenuHeart(posOnButton);

                if (fightButtonRenderer != null && fightHighlightSprite != null)
                    fightButtonRenderer.sprite = fightHighlightSprite;

                if (dialogueText != null)
                    dialogueText.text = $"* 영웅이 나타났다";
                break;

            case BattleState.EnemySelect:
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);

                enemySelectPanel.SetActive(true);
                ActivateMenuHeart(posOnSelect);
                break;

            case BattleState.Attack:
                dialogueBox.SetActive(true);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                enemySelectPanel.SetActive(false);

                attackPanel.SetActive(true);

                if (attackCursor != null)
                {
                    attackCursor.localPosition = new Vector3(-attackWidth / 2, 0, -5);
                    isCursorMovingRight = true;
                }
                break;

            case BattleState.EnemyTurn:
                dialogueBox.SetActive(false);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                enemySelectPanel.SetActive(false);
                StartCoroutine(EnemyPattern_co());
                break;
        }
    }

    void ActivateMenuHeart(Vector3 localPos)
    {
        GameObject heartToUse = isGreenMode ? menuGreenHeart : menuRedHeart;
        if (heartToUse != null)
        {
            heartToUse.SetActive(true);
            heartToUse.transform.localPosition = new Vector3(localPos.x, localPos.y, -10f);
        }
    }

    IEnumerator EnemyPattern_co()
    {
        int boxIndex = 0;
        bool useGreenCombat = false;

        if (isGreenMode)
        {
            boxIndex = 0;
            useGreenCombat = true;
        }
        else
        {
            int rnd = Random.Range(0, 2);
            boxIndex = (rnd == 0) ? 1 : 2;
            useGreenCombat = false;
        }

        if (battleBoxes != null && battleBoxes.Length > boxIndex)
        {
            GameObject activeBox = battleBoxes[boxIndex];
            activeBox.SetActive(true);

            BoxCollider2D boxCol = activeBox.GetComponent<BoxCollider2D>();
            if (boxCol != null && !useGreenCombat)
            {
                combatRedHeart.GetComponent<HeartController>().SetBoundaries(boxCol.bounds);
            }
        }

        GameObject combatHeart = useGreenCombat ? combatGreenHeart : combatRedHeart;
        if (combatHeart != null)
        {
            combatHeart.SetActive(true);

            combatHeart.transform.position = new Vector3(0f, 0f, -10f);

            if (!useGreenCombat)
                combatHeart.GetComponent<HeartController>().SetBattleMode(true);
        }

        if (patternManager != null)
        {
            patternManager.SetTarget(combatHeart.transform);
        }

        yield return new WaitForSeconds(0.5f);

        if (patternManager != null)
        {
            if (isGreenMode) patternManager.StartShieldPattern();
            else
            {
                if (boxIndex == 1) patternManager.StartCirclePattern();
                else patternManager.StartRandomPattern();
            }
        }

        yield return new WaitForSeconds(5.0f);

        if (patternManager != null) patternManager.StopMonsterAttack();

        if (!useGreenCombat && combatRedHeart != null)
            combatRedHeart.GetComponent<HeartController>().SetBattleMode(false);

        isGreenMode = !isGreenMode;

        ChangeState(BattleState.Dialogue);
    }

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
            StartCoroutine(GameClear_co());
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

    IEnumerator GameClear_co()
    {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("GameClear");
    }
}