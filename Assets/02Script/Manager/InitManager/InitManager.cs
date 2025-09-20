using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 모든 InitManager들의 공통 부모 클래스
/// </summary>

public class InitManager : MonoBehaviour
{
    protected DatabaseManager db;

    // 인벤토리
    protected InventoryManager invenManager;
    protected InventoryDescription invenDesc;
    protected InventorySlot invenSlot;
    protected InventoryUI invenUI;

    protected virtual void Start()
    {
        db = DatabaseManager.Instance;
        invenManager = InventoryManager.Instance;

        // 씬에 존재하는 IActionItem 찾아 Init 실행
        var items = FindObjectsOfType<MonoBehaviour>().OfType<IActionItem>();
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

        invenDesc = FindAnyObjectByType<InventoryDescription>();
        if (invenDesc == null) Debug.Log("InitManager - Failed to Load InvenDescription");

        invenUI = FindAnyObjectByType<InventoryUI>();
        if (invenUI == null) Debug.Log("InitManager - Failed to Load InventoryUI");

        //invenSlot = FindAnyObjectByType<InventorySlot>();
        //if (invenSlot == null) Debug.Log("InitManager - Failed to Load InvenSlot");

        // 인벤토리 Init
        invenManager?.Init(db);
        invenDesc?.Init(db);
        invenSlot?.Init(db);   //씬시작시 동적으로 만들어짐
        invenUI?.Init(db);
    }
}
