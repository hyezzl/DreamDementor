using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FilterType
{ 
    OldMovie,           // 튜토리얼 자체효과
    Glitch,             // 대사 중간중간 / 1인칭 맵에서 귀신과의 거리가 가까워질 때
    HorrorReverse,      // Red White / 귀신이 등장해 있을 때
}


public class FilterManager : MonoBehaviour
{
    //[Header("Filter Refs")]
    //[SerializeField] 

    protected Camera mainCam;

    protected virtual void Awake() { 
        mainCam = Camera.main;
    }

    protected void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.FilterOn>(CamFilterOn);
        //EventBus.Instance.Subscribe<GameEvents>
    }
    protected void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.FilterOn>(CamFilterOn);
    }

    protected virtual void CamFilterOn(GameEvents.FilterOn evt) { 
        // 공통 기능 정의
    }


    // duration값이 있는 필터 적용 함수 (시간만 처리)
    //protected IEnumerator FiniteFilter(float duration, System.Action onStart, System.Action onEnd) {
    //    onStart?.Invoke();
    //    yield return new WaitForSeconds(duration);
    //    onEnd?.Invoke();
    //}

    protected IEnumerator FiniteFilter(ICameraFilter filter, float duration) {
        filter.enabled = true;
        yield return new WaitForSeconds(duration);
        filter.enabled = false;
    }
}
