using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private InteractableData data;
    private IDatabase database;
    private IReadOnlyList<ItemInstance> inventory;

    public int GetItemID() => itemID;
    public ItemType GetItemType() => data.type;

    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetInteractable(itemID);
        if (data == null) Debug.Log($"Interactable : Failed to Load Item {itemID}");
    }

    public virtual void Interact()
    {
        // Monologue 출력
        EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.monologue));
    }

    //public int CountPairID(string pairID)
    //{
    //    return inventory.Count(item => item.pairID == pairID);
    //}

    //private void CheckInventory() {
    //    // inventorManager 접근 > GetInventory() 받아와서 갯수 셈
    //    inventory = InventoryManager.Instance.GetInventory();

    //    // 갯수 세기
    //    if (CountPairID(data.pairID) == data.hnum)
    //    {
    //        // ActiveMSG
    //        EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.activeMSG));
    //        // MiniSelection
    //        // todo :: 이벤트 생성 / 발행
    //    }
    //    else return;

    //}

    public GameObject GetObject()
    {
        return this.gameObject;
    }
}
