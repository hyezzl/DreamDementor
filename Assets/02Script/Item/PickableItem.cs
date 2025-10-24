using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private PickableData data;
    private IDatabase database;

    public int GetItemID() => itemID;
    public ItemType GetItemType() => data.type;

    public void Init(IDatabase db) {
        database = db;
        data = database.GetPickable(itemID);
        if (data == null) Debug.Log($"Pickable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        // 1. æ∆¿Ã≈€ Ω¿µÊ
        Pickup();

        // 2. Reply Dialog
        EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.reply));
    }

    public void Pickup() {
        EventBus.Instance.Publish<GameEvents.GetItem>(new GameEvents.GetItem(this));
        StartCoroutine(SafeDestroy());
    }

    IEnumerator SafeDestroy() { 
        yield return null;
        Destroy(gameObject);
    }
}
