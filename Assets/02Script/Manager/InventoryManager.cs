using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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

        // 임시
        //AddItem(ItemType.Pickable, 10001002);
        //AddItem(ItemType.Pickable, 10001003);
        //AddItem(ItemType.Pickable, 10001004);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GetItem>(OnGetItem);
        EventBus.Instance.Subscribe<GameEvents.PutItem>(OnPutItem);
        EventBus.Instance.Subscribe<GameEvents.UseItem>(OnUseItem);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GetItem>(OnGetItem);
        EventBus.Instance.Unsubscribe<GameEvents.PutItem>(OnPutItem);
        EventBus.Instance.Unsubscribe<GameEvents.UseItem>(OnUseItem);
    }
    private void OnGetItem(GameEvents.GetItem evt) {
        AddItem(evt.item.GetItemType(), evt.item.GetItemID());
    }

    private void OnPutItem(GameEvents.PutItem evt) {
        AddItem(evt.type, evt.itemID);
    }

    private void OnUseItem(GameEvents.UseItem evt) {
        RemoveItem(evt.itemID);
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
                Debug.Log($"{itemID}가 정상적으로 추가됨!");
                EventBus.Instance.Publish<UIEvents.InventoryChanged>(new UIEvents.InventoryChanged());
                
                break;

            case ItemType.Eatable:
                EatableData dataE = database.GetEatable(itemID);
                if (dataE == null)
                {
                    Debug.Log($"{itemID} : unknown EatableItem ERROR");
                    return;
                }
                Debug.Log($"{itemID}가 정상적으로 추가됨!");
                inventory.Add(new ItemInstance(itemID, ""));
                EventBus.Instance.Publish<UIEvents.InventoryChanged>(new UIEvents.InventoryChanged());

                break;
        }
    }

    // 아이템 사용 시, 지워지는 함수
    private void RemoveItem(int itemID) {
        // 조건에 맞는 첫 번째 ItemInstance 찾기
        var item = inventory.FirstOrDefault(i => i.itemID == itemID);
        if (item != null)
        {
            inventory.Remove(item);
            EventBus.Instance.Publish<UIEvents.InventoryChanged>(new UIEvents.InventoryChanged());
            Debug.Log($"{itemID} 삭제");
        }
        else {
            Debug.Log($"{itemID} 가 인벤토리 내에 없음");
        }
    }


    public IReadOnlyList<ItemInstance> GetInventory() => inventory.AsReadOnly();
}
