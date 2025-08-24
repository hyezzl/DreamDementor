using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    // 플레이어가 가진 아이템
    private List<ItemInstance> inventory = new();
    private IDatabase database;

    // 초기화 함수
    public void Init(IDatabase newDB)
    {
        database = newDB;
    }

    public void AddItem(int itemID, IDatabase db)
    {
        // 필요한 정보
        PickableData data = db.GetPickable(itemID);
        if (data == null)
        {
            Debug.Log($"{itemID} : unknown item ERROR");
            return;
        }
        // 아이템 인벤토리에 추가 : todo : 이벤트 발행
        inventory.Add(new ItemInstance(itemID));
    }

    // todo : 아이템 사용 시, 지워지는 함수


    public IReadOnlyList<ItemInstance> GetInventory() => inventory.AsReadOnly();
}
