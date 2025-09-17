using System.Collections;
using UnityEngine;

public class EatableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private EatableData data;
    private IDatabase database;
    public int GetItemID() => itemID;
    public ItemType GetItemType() => data.type;

    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetEatable(itemID);
        if (data == null) Debug.Log($"Eatable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        Debug.Log($"Eatable -  {data.itemName} : {data.description}");
        // 1. 아이템 주움
        Pickup();

        // 2. Reply 재생
        EventBus.Instance.Publish<UIEvents.OpenMonologue>(new UIEvents.OpenMonologue(itemID, data.reply));
    }

    public void Pickup()
    {
        EventBus.Instance.Publish<GameEvents.GetItem>(new GameEvents.GetItem(this));
        StartCoroutine(SafeDestroy());
    }

    IEnumerator SafeDestroy()
    {
        yield return null;
        Destroy(gameObject);
    }
}
