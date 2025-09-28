using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IActionNpc
{
    public string npcID;
    public bool isContacted = false;
    private Dictionary<int, NPCDialogData> dialogDatas;
    private NPCReDialogData reDialogData;
    private IDatabase database;

    private bool isInDialog = false;

    public string GetNpcID() => npcID;

    public void Init(IDatabase db)
    {
        database = db;
        dialogDatas = database.GetNpcDialog(npcID);
        if (dialogDatas == null) Debug.Log("NPC - Failed to Load NpcDialogs");
        reDialogData = database.GetNpcReDialog(npcID);
        if (reDialogData == null) Debug.Log("NPC - Failed to Load NpcReDialogs");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }

    public void Interact()
    {
        if (isInDialog) return; // 대화중이면 무시

        if (!isContacted)
        {
            // 첫 대면 : 대화
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, dialogDatas));
            isContacted = true;
        }
        else {
            // 재 대화시 재대화
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        if (evt.npcID == npcID) {
            isInDialog = false;
        }
    }

}
