using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 텍스트 매쉬 프로를 쓰려면 이게 필요해!
using UnityEngine.EventSystems; // 마우스 이벤트를 감지하는 도구상자야!

// [유니] 마우스 이벤트를 받기 위해 인터페이스(약속) 2개를 추가했어!
// 1. IPointerEnterHandler: 마우스가 들어왔을 때
// 2. IPointerExitHandler: 마우스가 나갔을 때
public class TextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("설정")]
    public Color highlightColor = Color.yellow; // 마우스 올렸을 때 바뀔 색
    private Color originalColor; // 원래 색 기억용

    private TMP_Text myText;

    private void Awake()
    {
        // 내 몸에 붙어있는 텍스트 컴포넌트 가져오기
        TryGetComponent(out myText);

        // 시작할 때 원래 색상을 기억해두자!
        if (myText != null)
        {
            originalColor = myText.color;
        }
    }

    // [유니] 마우스가 글자 위로 들어왔을 때 발동!
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myText != null)
        {
            myText.color = highlightColor; // 색상 변경!
        }
    }

    // [유니] 마우스가 글자 밖으로 나갔을 때 발동!
    public void OnPointerExit(PointerEventData eventData)
    {
        if (myText != null)
        {
            myText.color = originalColor; // 원래 색으로 복구!
        }
    }
}