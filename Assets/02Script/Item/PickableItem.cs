using UnityEngine;

public class PickableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private PickableData data;
    private IDatabase database;

    public int GetItemID() => itemID;

    public void Init(IDatabase db) {
        database = db;
        data = database.GetPickable(itemID);
        if (data == null) Debug.Log($"Pickable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        Debug.Log($"{data.itemName} : {data.description}");
        // 1.아이템 습득

        // 2. Reply Dialog // 시간

    }
}
