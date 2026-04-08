using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

    private void Start()
    {
        // 모든 Zone 객체 찾기
        var triggerZones = FindObjectsOfType<MonoBehaviour>().OfType<ITriggerZone>();

        foreach (var zone in triggerZones) {
            // 씬 내 Zone 활성화/비활성화 갱신
            var zoneObject = (zone as MonoBehaviour).gameObject;
            bool isActive = EventHistoryManager.Instance.GetZoneActive(zone.ZoneID);
            Debug.Log($"[데이터 수동 체크] ID: {zone.ZoneID} | 결과값: {isActive}");

            zoneObject.SetActive(isActive);
        }
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
        EventHistoryManager.Instance.SetZoneActive(evt.zoneID, evt.isActive);
    }
}
