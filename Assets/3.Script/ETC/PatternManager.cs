using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private GameObject circleSpearPrefab;
    [SerializeField] private GameObject randomSpearPrefab;

    // [유니] 오빠가 만든 4개의 스폰 위치를 여기에 순서대로 넣어줘!
    // 순서 중요: [0:위], [1:아래], [2:왼쪽], [3:오른쪽]
    [SerializeField] private Transform[] spawnPoints; 

    private List<SpearController> activeSpears = new List<SpearController>();
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        UpdateNearestSpear();

        // [유니] 테스트용 치트키! 스페이스바를 누르면 랜덤 소환!
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StartCirclePattern();
            StartRandomPattern();
            //int randomDir = Random.Range(0, 4);
            //bool randomTrick = Random.Range(0, 2) == 0; // 50% 확률
            //SpawnSpear(randomDir, randomTrick);
        }
    }

    public void StartPattern(EnemyData data, Vector3 pos)
    {
        StartCoroutine(SpawnSpears_co());
    }

    public void StopMonsterAttack()
    {
        StopAllCoroutines();
        foreach (var spear in activeSpears)
        {
            if (spear != null) Destroy(spear.gameObject);
        }
        activeSpears.Clear();
    }

    IEnumerator SpawnSpears_co()
    {
        while (true)
        {
            int direction = Random.Range(0, 4);
            bool isTrick = Random.Range(0, 10) < 3; // 30% 확률

            SpawnSpear(direction, isTrick);
            yield return new WaitForSeconds(0.8f);
        }
    }

    void SpawnSpear(int dir, bool isTrick)
    {
        if (spawnPoints == null || spawnPoints.Length < 4) return;

        Vector3 rawPos = spawnPoints[dir].position;

        float playerZ = playerTransform.position.z;
        Vector3 spawnPos = new Vector3(rawPos.x, rawPos.y, playerZ);

        Quaternion rotation = Quaternion.identity;

        // [유니] "오른쪽을 보는 이미지" 기준 회전값 설정
        // isTrick이면 정반대로 돌려버리기! (180도 추가)
        float trickOffset = isTrick ? 180f : 0f;

        switch (dir)
        {
            case 0: // 위에서 등장 (아래를 봐야 함: -90도)
                rotation = Quaternion.Euler(0, 0, -90 + trickOffset); 
                break;
            case 1: // 아래에서 등장 (위를 봐야 함: 90도)
                rotation = Quaternion.Euler(0, 0, 90 + trickOffset);
                break;
            case 2: // 왼쪽에서 등장 (오른쪽을 봐야 함: 0도)
                rotation = Quaternion.Euler(0, 0, 0 + trickOffset);
                break;
            case 3: // 오른쪽에서 등장 (왼쪽을 봐야 함: 180도)
                rotation = Quaternion.Euler(0, 0, 180 + trickOffset);
                break;
        }

        GameObject obj = Instantiate(spearPrefab, spawnPos, rotation);
        
        SpearController spear = obj.GetComponent<SpearController>();
        spear.Init(isTrick, playerTransform); 

        activeSpears.Add(spear);
    }

    void UpdateNearestSpear()
    {
        if (playerTransform == null || activeSpears.Count == 0) return;

        SpearController nearest = null;
        float minDist = float.MaxValue;

        for (int i = activeSpears.Count - 1; i >= 0; i--)
        {
            if (activeSpears[i] == null)
            {
                activeSpears.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(activeSpears[i].transform.position, playerTransform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = activeSpears[i];
            }
        }

        foreach (var spear in activeSpears)
        {
            if (spear == nearest) spear.SetNearest(true);
            else spear.SetNearest(false);
        }
    }

    public void StartCirclePattern()
    {
        StartCoroutine(SpawnCircleSpears_co());
    }

    IEnumerator SpawnCircleSpears_co()
    {
        float currentDelay = 1f; // 처음엔 느긋하게
        float radius = 4.0f;       // 원의 크기

        while (true)
        {
            // 플레이어 위치 (없으면 0,0)
            Vector3 centerPos = playerTransform != null ? playerTransform.position : Vector3.zero;

            // [유니] Z값 보정 (플레이어랑 똑같은 높이!)
            float zPos = centerPos.z;

            // 6개 창 소환
            for (int i = 0; i < 6; i++)
            {
                // 각도 계산 (60도 간격 + 랜덤 회전)
                float angle = (i * 60) + Random.Range(0f, 30f);

                // 원형 좌표 (수학 시간!)
                float x = centerPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
                float y = centerPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
                Vector3 spawnPos = new Vector3(x, y, zPos);

                // 회전: 중심(플레이어)을 바라보게!
                // (오빠 화살표가 오른쪽을 본다면 -90도나 +90도 보정 필요할 수 있음)
                Quaternion rotation = Quaternion.Euler(0, 0, angle + 90);

                GameObject obj = Instantiate(circleSpearPrefab, spawnPos, rotation);

                // 새 스크립트 초기화!
                obj.GetComponent<CircleSpearController>().Init(currentDelay);
            }

            // 다음 웨이브 대기
            yield return new WaitForSeconds(currentDelay + 0.5f);

            // 점점 빨라지게! (난이도 상승)
            currentDelay = Mathf.Max(0.1f, currentDelay - 0.3f);
        }
    }

    public void StartRandomPattern()
    {
        StartCoroutine(SpawnRandomSpears_co());
    }

    IEnumerator SpawnRandomSpears_co()
    {
        float currentDelay = 1.2f; // 초기 대기 시간
        float minDelay = 0.4f;     // 최소 대기 시간
        float spawnInterval = 0.8f; // 소환 간격 (점점 빨라짐)

        while (true)
        {
            // 플레이어 위치 기준
            Vector3 centerPos = playerTransform != null ? playerTransform.position : Vector3.zero;

            // 랜덤 위치 계산 (거리 4~6 사이)
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(4.0f, 6.0f);

            float x = centerPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float y = centerPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

            // Z값은 플레이어와 동일하게! (중요 )
            Vector3 spawnPos = new Vector3(x, y, centerPos.z);

            GameObject obj = Instantiate(randomSpearPrefab, spawnPos, Quaternion.identity);

            // 초기화 (타겟, 딜레이 전달)
            obj.GetComponent<RandomSpearController>().Init(playerTransform, currentDelay);

            // 다음 소환까지 대기
            yield return new WaitForSeconds(spawnInterval);

            // 난이도 상승! (반응 속도 & 소환 속도 둘 다 빨라짐)
            currentDelay = Mathf.Max(minDelay, currentDelay - 0.05f);
            spawnInterval = Mathf.Max(0.2f, spawnInterval - 0.05f);
        }
    }
}