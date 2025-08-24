using System.Linq;
using UnityEngine;

/// <summary>
/// 모든 InitManager들의 공통 부모 클래스
/// </summary>

public class InitManager : MonoBehaviour
{
    protected DatabaseManager db;
    protected InventoryManager invenManager;

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

        // 인벤토리 Init
        invenManager.Init(db);
    }
}
