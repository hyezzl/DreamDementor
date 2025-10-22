using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

/// <summary>
/// 인벤토리 관리 / 아이템 습득
/// </summary>

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

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GetItem>(OnGetItem);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GetItem>(OnGetItem);
    }
    private void OnGetItem(GameEvents.GetItem evt) {
        AddItem(evt.item.GetItemType(), evt.item.GetItemID());
    }


    public void AddItem(ItemType type, int itemID)
    {
        // 필요한 정보
        switch (type) {
            case ItemType.Pickable:
                PickableData dataP = database.GetPickable(itemID);
                if (dataP == null)
                {
                    Debug.Log($"{itemID} : unknown PickableItem ERROR");
                    return;
                }
                inventory.Add(new ItemInstance(itemID, dataP.pairID));
                EventBus.Instance.Publish<UIEvents.InventoryChanged>(new UIEvents.InventoryChanged());
                
                break;

            case ItemType.Eatable:
                EatableData dataE = database.GetEatable(itemID);
                if (dataE == null)
                {
                    Debug.Log($"{itemID} : unknown EatableItem ERROR");
                    return;
                }
                inventory.Add(new ItemInstance(itemID, ""));
                EventBus.Instance.Publish<UIEvents.InventoryChanged>(new UIEvents.InventoryChanged());

                break;
        }
    }

    // todo : 아이템 사용 시, 지워지는 함수
    private void RemoveItem(int itemID) {
        inventory.Remove(new ItemInstance(itemID, ""));
        //if (item.uniqueID != 0) { 
            
        //}
    }


    public IReadOnlyList<ItemInstance> GetInventory() => inventory.AsReadOnly();
}
