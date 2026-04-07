using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메인빌런과의 충돌처리 : 즉시 사망
/// </summary>
public class EnemyCrash : MonoBehaviour
{
    [Header("Scene Info")]
    [SerializeField] private SceneType curScene;

    [Header("Death Setting")]
    [SerializeField] private DeathType deathType = DeathType.CrashEnemy;
    [SerializeField] private bool isDead = false;    // 중복방지

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("게임오버! 플레이어 충돌");
            isDead = true;

            // 게임오버 이벤트 발행
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.CrashEnemy));

            // 게임모드
            PlayerController.Instance.CurMode = GameMode.GameOverMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));
        }
    }
    // 외부 호출
    public void ReStart() { 
        isDead = false;
    }
}
