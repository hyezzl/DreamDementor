using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderData;

// 대화 이후 이벤트 강제 진행 (Conversation한정)

public class EventNPC : NPC
{
    public string eventID;
    public bool isEndEvent = false;

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


    public override void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (!isEndEvent)
        {
            // 이벤트 발생 전
            isInDialog = true;

            // 미니선택지 나옴

            //EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, npcID, initialDialog));
            isContacted = true;
        }
        else
        {
            // 이벤트 발생 이후
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    // 미니 선택지 따라 이벤트 발행 / 대화끝


    protected override void OnEndNpcDialog(UIEvents.EndNpcDialog evt)
    {
        base.OnEndNpcDialog(evt);
        if (evt.npcID == npcID && evt.npcEventID == curNpcEventID) {
            // 강제 이벤트 발행
            ContinueEvent();

            isEndEvent = true;
        }
    }

    protected virtual void ContinueEvent() { }
}
