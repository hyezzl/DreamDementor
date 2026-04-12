using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 괴물 시야 콜라이더
/// </summary>
public class EnemyVision : MonoBehaviour
{
    private EnemyMove em;
    private void Awake()
    {
        em = GetComponentInParent<EnemyMove>();
        if (em == null) Debug.Log("EnemyVision - Failed to Load EnemyMove");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            MeshRenderer playerMr = other.GetComponent<MeshRenderer>();

            // 캐비닛안이라면 (플레이어의 메쉬가 꺼져있다면)
            if (playerMr != null && !playerMr.enabled)
            {
                return;
            }

            em.SetPlayerInVision(true);

            if (em.curState == EnemyState.Patrol)
            {
                EventBus.Instance.Publish(new GameEvents.EnemyStateChange(EnemyState.Chase));
            }
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            em.SetPlayerInVision(false);
            Debug.Log("나감");
        } 
    }

}
