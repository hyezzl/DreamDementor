using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 피가 30이하일 때 
/// </summary>
public class ReverseModeManager : MonoBehaviour
{


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnLackedHP>(OnWarningState);
        EventBus.Instance.Subscribe<GameEvents.OnSteadyHp>(OnSteadyState);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnLackedHP>(OnWarningState);
        EventBus.Instance.Unsubscribe<GameEvents.OnSteadyHp>(OnSteadyState);
    }


    // 정신력 부족으로 인한 반전 상태 개시
    private void OnWarningState(GameEvents.OnLackedHP evt) { 
        
    }

    // 정신력 회복으로 반전 상태 종료
    private void OnSteadyState(GameEvents.OnSteadyHp evt) { 
    
    }
}
