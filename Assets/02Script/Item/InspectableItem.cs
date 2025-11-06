using UnityEngine;

public class InspectableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private InspectableData data;
    private IDatabase database;


    public int GetItemID() => itemID;
    public ItemType GetItemType() => data.type;


    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetInspectable(itemID);
        if (data == null) Debug.Log($"Inspectable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        // Monologue Ãâ·Â
        EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.monologue));
    }

    public GameObject GetObject()
    {
        return this.gameObject;
    }

}
