using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision_2F : MonoBehaviour
{
    private Horror02_Enemy em;
    private void Awake()
    {
        em = GetComponentInParent<Horror02_Enemy>();
        if (em == null) Debug.Log("EnemyVision2F - Failed to Load EnemyMove");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            em.SetPlayerInVision(true);
            if (em.curState == EnemyState.Patrol)
            {
                MeshRenderer playerMr = other.GetComponent<MeshRenderer>();

                // 캐비닛안이라면 (플레이어의 메쉬가 꺼져있다면)
                if (playerMr != null && !playerMr.enabled)
                {
                    return;
                }
                else { 
                    EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Chase));
                    // 와 동시에 게임오버
                    Debug.Log("캐비닛 밖으로 나와서 사망");
                    EventBus.Instance.Publish<GameEvents.GameOver>(new GameEvents.GameOver(SceneType.HorrorScene, DeathType.CrashEnemy));

                    // 이벤트 모드 발동 (움직임 제어)
                    PlayerController.Instance.CurMode = GameMode.EventMode;
                    EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));
                }

                
            }
        }
    }
}
