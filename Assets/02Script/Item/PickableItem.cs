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
        Debug.Log($"Pickable - {data.itemName} : {data.description}");
        // 1. æ∆¿Ã≈€ Ω¿µÊ

        // 2. Reply Dialog
        EventBus.Instance.Publish<UIEvents.OpenMonologue>(new UIEvents.OpenMonologue(itemID, data.reply));
    }
}
