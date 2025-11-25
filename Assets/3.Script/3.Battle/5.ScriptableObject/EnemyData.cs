using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string EnemyName; // 보스 이름 (예: Asgore)
    public int MaxHP = 10;   // [유니] 오빠 요청대로 체력 10!
    public int CurrentHP;

    [Header("대사")]
    [TextArea] public string EncounterDialogue; // 조우 시 대사

    // [유니] 전투 시작할 때 체력을 꽉 채워주는 함수
    public void CalEnemyHP()
    {
        CurrentHP = MaxHP;
    }

    // [유니] 데미지 입는 함수
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;
    }
}