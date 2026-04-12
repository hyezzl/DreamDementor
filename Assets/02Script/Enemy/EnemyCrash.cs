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

    private EnemyMove em;

    private void Awake()
    {
        // 귀신의 현재 상태를 알기 위해 EnemyMove 참조
        em = GetComponent<EnemyMove>();
        if (em == null) Debug.Log("EnemyCrash - Failed to Load EnemyMove");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if (em.curState == EnemyState.Patrol) return;

            // 플레이어가 캐비닛에 은신 중이면 충돌 무시
            MeshRenderer playerMr = other.GetComponent<MeshRenderer>();
            if (playerMr != null && !playerMr.enabled)
            {
                Debug.Log("캐비닛! 안전!");
                return;
            }

            //if (em.curState == EnemyState.Chase || em.curState == EnemyState.LostTarget)
            //{
            //    ExecuteGameOver();
            //}
            //ExecuteGameOver();
            Debug.Log("게임오버! 플레이어 충돌");
        }
    }

    private void ExecuteGameOver()
    {
        Debug.Log("게임오버! 플레이어 충돌");
        isDead = true;

        // 게임오버 이벤트 발행
        EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(curScene, DeathType.CrashEnemy));

        // 게임모드
        PlayerController.Instance.CurMode = GameMode.GameOverMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));
    }

    // 외부 호출
    public void ReStart() { 
        isDead = false;
    }
}
