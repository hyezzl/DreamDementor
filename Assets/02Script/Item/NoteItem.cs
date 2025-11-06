using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private NoteData data;
    private IDatabase database;

    public int GetItemID() => itemID;
    public ItemType GetItemType() => data.type;

    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetNote(itemID);
        if (data == null) Debug.Log($"Pickable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        // 1. 아이템 습득
        Pickup();

        // 2. Reply Dialog
        EventBus.Instance.Publish<UIEvents.ItemMonologue>(new UIEvents.ItemMonologue(itemID, data.reply));
    }

    public void Pickup()
    {
        // 메모얻은 이벤트
        EventBus.Instance.Publish<GameEvents.GetNote>(new GameEvents.GetNote(itemID, data.text));

        StartCoroutine(SafeDestroy());
    }

    IEnumerator SafeDestroy()
    {
        yield return null;
        Destroy(gameObject);
    }

    public GameObject GetObject()
    {
        return this.gameObject;
    }
}
