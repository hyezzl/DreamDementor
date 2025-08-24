using UnityEngine;

public class ReadableItem : MonoBehaviour, IActionItem
{
    public int itemID;
    private bool isRead = false;  // 한번이라도 읽었는지 (고유값)
    private ReadableData data;
    private IDatabase database;

    public int GetItemID() => itemID;

    public void Init(IDatabase db)
    {
        database = db;
        data = database.GetReadable(itemID);
        if (data == null) Debug.Log($"Readable : Failed to Load Item {itemID}");
    }

    public void Interact()
    {
        if (!isRead) // 처음읽었을 때
        {
            Debug.Log($"{data.itemName} : {data.narrative}");
        }
        else { 
            //선택지
        }

    }

}
