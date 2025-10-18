using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class NPC : MonoBehaviour, IActionNpc
{
    public string npcID;
    public bool isContacted = false;
    private Dictionary<string, Dictionary<int, NPCDialogData>> allDialogs;
    private Dictionary<int, NPCDialogData> initialDialog;
    private NPCReDialogData reDialogData;
    private IDatabase database;

    private bool isInDialog = false;

    public string GetNpcID() => npcID;

    public void Init(IDatabase db)
    {
        database = db;
        // 해당 npc의 전체 대화 이벤트 가져오기
        allDialogs = database.GetNpcEvent(npcID);
        if (allDialogs == null) Debug.Log("NPC - Failed to Load NpcDialogs");
        initialDialog = database.GetNpcDialog(npcID, npcID);
        if (initialDialog == null) Debug.Log("NPC - Failed to Load NpcDialogs");
        reDialogData = database.GetNpcReDialog(npcID);
        if (reDialogData == null) Debug.Log("NPC - Failed to Load NpcReDialogs");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }

    public void Interact()
    {
        if (isInDialog) return; // 대화중이면 무시

        if (!isContacted)
        {
            // 첫 대면 : 대화
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, initialDialog));
            isContacted = true;
        }
        else {
            // 재 대화시 재대화
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    public void OnNpcChoice(UIEvents.MakeChoice evt) {
        // npc선택지일때만
        if (evt.isNpc) {
            // ChoiceData에서 continue꺼냄
            string continueID = null;
            if (evt.data != null && evt.data.continueIDs.Count > evt.selectIdx)
            {
                continueID = evt.data.continueIDs[evt.selectIdx];
            }

            if (!string.IsNullOrEmpty(continueID))
            {
                if (allDialogs.TryGetValue(continueID, out var branchDialog))
                {
                    // 선택지 이후 분기대화
                    EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, branchDialog));
                }
                else { Debug.Log($"{continueID}에 대한 분기대화 없음"); }
            }
            else { 
                // 선택지는 있으나, 그 후 이벤트 없을경우
            }
        }
    }

    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        if (evt.npcID == npcID) {
            isInDialog = false;
        }
    }

}
