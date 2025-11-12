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
    [SerializeField] private bool isDead = true;    // 중복방지

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDead)
        {
            Debug.Log("게임오버! 플레이어 충돌");
            isDead = true;
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.CrashEnemy));

            // 게임모드
            PlayerController.Instance.CurMode = GameMode.GameOverMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));
        }
        else if (other.CompareTag("Player") && isDead) {
            Debug.Log("테스트용! 게임오버됨");
            EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.CrashEnemy));
        }
    }
}
