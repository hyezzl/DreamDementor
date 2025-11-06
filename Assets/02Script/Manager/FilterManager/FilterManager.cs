using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FilterType
{ 
    OldMovie,           // 튜토리얼 자체효과
    Glitch,             // 대사 중간중간 / 1인칭 맵에서 귀신과의 거리가 가까워질 때
    HorrorReverse,      // Red White / 귀신이 등장해 있을 때
    Spot,               // 씬 전환효과
}


public class FilterManager : MonoBehaviour
{

    protected Camera mainCam;
    protected virtual void Awake() {
        mainCam = Camera.main;
    }

    // 사용할 필터 모두 정의
    public Dictionary<FilterType, Type> filterMap = new Dictionary<FilterType, Type>()
    {
        { FilterType.OldMovie,      typeof(CameraFilterPack_TV_Old_Movie) },
        { FilterType.Glitch,        typeof(CameraFilterPack_FX_Glitch1) },
        { FilterType.HorrorReverse, typeof(CameraFilterPack_Colors_Adjust_PreFilters) },
        { FilterType.Spot,          typeof(CameraFilterPack_FX_Spot) },
    };


    protected void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.FilterOn>(CamFilterOn);
        EventBus.Instance.Subscribe<GameEvents.FilterOff>(CamFilterOff);
    }
    protected void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.FilterOn>(CamFilterOn);
        EventBus.Instance.Subscribe<GameEvents.FilterOff>(CamFilterOff);
    }

    protected virtual void CamFilterOn(GameEvents.FilterOn evt) {
        var filter = FindFilter(evt.type);
        if (filter == null) return;

        // 무한 필터
        if (evt.isLasting)
        {
            InfiniteFilterOn(filter);
        }
        // 유한 필터
        else 
        {
            StartCoroutine(FiniteFilterOn(filter, evt.duration));
        }
    }

    protected virtual void CamFilterOff(GameEvents.FilterOff evt) {
        var filter = FindFilter(evt.type);
        if (filter == null) return;

        FilterOff(filter);
    }

    // filter 찾음 (메인카메라에서 실시간으로 찾음)
    protected virtual ICameraFilter FindFilter(FilterType type) {
        if (filterMap.TryGetValue(type, out var filterType)) {
            var filter = Camera.main.GetComponent(filterType) as ICameraFilter;
            if (filter != null) {
                return filter;    
            }
        }
        return null;
    }


    // 유한 필터
    protected IEnumerator FiniteFilterOn(ICameraFilter filter, float duration) {
        filter.enabled = true;
        yield return new WaitForSeconds(duration);
        filter.enabled = false;
    }


    // 무한 필터
    protected virtual void InfiniteFilterOn(ICameraFilter filter) {
        filter.enabled = true;
    }

    // 필터 삭제
    protected virtual void FilterOff(ICameraFilter filter) {
        filter.enabled = false;
    }
}
