using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IActionNpc
{
    public string npcID;
    private Dictionary<int, NPCDialogData> dialogDatas;
    private NPCReDialogData reDialogData;
    private IDatabase database;

    public string GetNpcID() => npcID;

    public void Init(IDatabase db)
    {
        database = db;
        dialogDatas = database.GetNpcDialog(npcID);
        reDialogData = database.GetNpcReDialog(npcID);
    }

    public void Interact()
    {
        // 대화가 시작
        Debug.Log("관찰됨");

        EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, dialogDatas));

        // 재 대화시 재대화
    }

}
