using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    // 존활성화 정보 관리
    public static Dictionary<string, bool> zoneActiveMap = new();

    private void Start()
    {
        // 모든 Zone 객체 찾기
        var triggerZones = FindObjectsOfType<MonoBehaviour>().OfType<ITriggerZone>();

        foreach (var zone in triggerZones) {
            // 상태값이 없다면 키값 추가 후 true 초기화
            if (!zoneActiveMap.ContainsKey(zone.ZoneID)) { 
                zoneActiveMap[zone.ZoneID] = true;
            }

            // 씬 내 Zone 활성화/비활성화 갱신
            var zoneObject = (zone as MonoBehaviour).gameObject;
            if (zoneActiveMap.TryGetValue(zone.ZoneID, out bool isActive))
            {
                zoneObject.SetActive(isActive);
            }
            else {
                zoneObject.SetActive(true);
            }
        }
        Debug.Log($"Zone 초기화 : {zoneActiveMap.Count}개 완료");

    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.ActiveZone>(ControlZone);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.ActiveZone>(ControlZone);
    }

    private void ControlZone(GameEvents.ActiveZone evt) {
        // 등록된 키
        if (zoneActiveMap.ContainsKey(evt.zoneID))
        {
            zoneActiveMap[evt.zoneID] = evt.isActive;
        }
        // 등록되지 않은 키
        else {
            zoneActiveMap[evt.zoneID] = evt.isActive;
        }
    }
}
