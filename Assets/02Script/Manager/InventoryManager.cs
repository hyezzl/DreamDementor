using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class InventoryManager : Singleton<InventoryManager>
{
    // 플레이어가 가진 아이템
    private List<ItemInstance> inventory = new();
    private IDatabase database;

    // 초기화 함수
    public void Init(IDatabase db)
    {
        database = db;
    }

    public void AddItem(ItemType type, int itemID, IDatabase db)
    {
        // 필요한 정보
        switch (type) {
            case ItemType.Pickable:
                PickableData dataP = db.GetPickable(itemID);
                if (dataP == null)
                {
                    Debug.Log($"{itemID} : unknown PickableItem ERROR");
                    return;
                }
                // 아이템 인벤토리에 추가 : todo : 이벤트 발행
                inventory.Add(new ItemInstance(itemID, dataP.pairID));
                break;

            case ItemType.Eatable:
                EatableData dataE = db.GetEatable(itemID);
                if (dataE == null)
                {
                    Debug.Log($"{itemID} : unknown EatableItem ERROR");
                    return;
                }
                // 아이템 인벤토리에 추가 : todo : 이벤트 발행
                inventory.Add(new ItemInstance(itemID, ""));
                break;
        }
    }

    // todo : 아이템 사용 시, 지워지는 함수


    //public int CountPairID(string pairID) {
    //    return inventory.Count(item => item.pairID == pairID);
    //}

    public IReadOnlyList<ItemInstance> GetInventory() => inventory.AsReadOnly();
}
