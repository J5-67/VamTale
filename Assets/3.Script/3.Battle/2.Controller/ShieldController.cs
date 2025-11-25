using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        TryGetComponent(out sr);
        if (sr != null) originalColor = sr.color;
    }

    void Update()
    {
        // [유니] 배틀 상태가 아니면 작동 안 하게 막아줘 (나중에 GameManager 연결 필요)
        // if (!GameManager.Instance.isLive) return; 

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, -90); // 위
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); // 아래
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // 왼쪽
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180); // 오른쪽
        }
    }

    // [유니 꿀팁] 방패가 활성화되면 플레이어는 못 움직이게 해야 해!
    // PlayerController에서 moveSpeed를 0으로 만들거나 입력을 막아야 해.

    // [유니] 창을 막았을 때 호출되는 함수!
    public void OnBlock()
    {
        if (sr != null)
        {
            StopAllCoroutines(); // 혹시 이미 깜빡이고 있다면 멈추고 새로 시작
            StartCoroutine(BlockEffect_co());
        }
    }

    IEnumerator BlockEffect_co()
    {
        sr.color = Color.red; // 빨간색으로 변신!
        yield return new WaitForSeconds(0.1f); // 0.1초 유지
        sr.color = originalColor; // 원래대로 복귀
    }
}