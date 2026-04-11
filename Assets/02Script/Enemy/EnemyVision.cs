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
        if (other.CompareTag("Player")) { 
            em.SetPlayerInVision(true);
            Debug.Log("들어옴");

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
