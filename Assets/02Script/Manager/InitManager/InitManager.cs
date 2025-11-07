using System.Linq;
using UnityEngine;

/// <summary>
/// 모든 InitManager들의 공통 부모 클래스
/// </summary>

public class InitManager : MonoBehaviour
{
    protected DatabaseManager db;

    protected virtual void Start()
    {
        db = DatabaseManager.Instance;


        // 씬에 존재하는 IActionItem / IActionNpc 찾아 Init 실행
        var items = FindObjectsOfType<MonoBehaviour>().OfType<IActionTarget>();
        if (items != null)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    item.Init(db);
                }
            }
        }

        // 씬에 존재하는 인벤토리관련 클래스 Init
        var invens = FindObjectsOfType<MonoBehaviour>().OfType<IInventory>();
        if (invens != null) {
            foreach (var inven in invens) { 
                if(inven != null) inven.Init(db);
            }
        }


        // 씬에 존재하는 TiggerZone찾아 이벤트 Init 실행
        var zones = FindObjectsOfType<MonoBehaviour>().OfType<ITriggerZone>();
        if (zones != null) { 
            foreach(var zone in zones){
                if (zones != null) { 
                    zone.Init(db);
                }
            }
        }

        // 씬에 존재하는 GameEvent 스크립트 찾아 Init 실행
        var events = FindObjectsOfType<MonoBehaviour>().OfType<IGameEvent>();
        if (events != null)
        {
            foreach (var evt in events)
            {
                if (events != null)
                {
                    evt.Init(db);
                }
            }
        }


        //invenDesc = FindAnyObjectByType<InventoryDescription>();
        //if (invenDesc == null) Debug.Log("InitManager - Failed to Load InvenDescription");

        //invenUI = FindAnyObjectByType<InventoryUI>();
        //if (invenUI == null) Debug.Log("InitManager - Failed to Load InventoryUI");

        ////invenSlot = FindAnyObjectByType<InventorySlot>();
        ////if (invenSlot == null) Debug.Log("InitManager - Failed to Load InvenSlot");

        //// 인벤토리 Init
        //invenManager?.Init(db);
        //invenDesc?.Init(db);
        //invenUI?.Init(db);
        //invenSlot?.Init(db);   //씬시작시 동적으로 만들어짐
    }
}
