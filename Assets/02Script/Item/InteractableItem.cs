using UnityEngine;

public class InteractableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private InteractableData data;
    private IDatabase database;

    public int GetItemID() => itemID;

    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetInteractable(itemID);
        if (data == null) Debug.Log($"Interactable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        Debug.Log($"{data.itemName} : {data.deactiveMSG}");
    }
}
