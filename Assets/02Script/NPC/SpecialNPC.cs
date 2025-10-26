using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 대화 이후 이벤트 강제 진행 (Conversation한정)

public class SpecialNPC : NPC
{
    public string eventID;

    protected Dictionary<string, Dictionary<int, DialogData>> allEventDialogs;
    protected Dictionary<int, DialogData> initialEventDialog;

    public override void Init(IDatabase db)
    {
        base.Init(db);

        // 진행할 이벤트의 정보도 가져옴
        allEventDialogs = database.GetDialogEvent(eventID);
        initialEventDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log($"*{this.GetType().Name} - Failed to Load DialogData");
    }

    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        base.OnEndNpcDialog(evt);
        if (evt.npcID == npcID && evt.npcEventID == curNpcEventID) {
            // 강제 이벤트 발행
            ContinueEvent();
        }
    }

    protected virtual void ContinueEvent() { }
}
