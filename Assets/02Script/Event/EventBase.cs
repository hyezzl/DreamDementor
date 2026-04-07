using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    Narration,
    Cutscene,
    Conversation,  // only 대화 (플레이어의 입력 포함)
    SceneChange,
    Intersection,
}

public class EventBase : MonoBehaviour, IGameEvent
{
    [Header("EventInfo")]
    public string eventID;
    public EventType eventtype;

    protected IDatabase database;
    protected PlayerController pc;
    protected Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    protected Dictionary<int, DialogData> initialDialog;
    protected List<NarrationData> narrations;

    protected virtual void Awake() {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log($"*{this.GetType().Name} - Failed to Load PlayerController");
    }

    // DB연결
    public virtual void Init(IDatabase db) {
        database = db;
        Debug.Log($"*{this.GetType().Name} 의 Init 완료!");

        switch (eventtype)
        {
            case EventType.Conversation:
                allDialogs = database.GetDialogEvent(eventID);
                initialDialog = database.GetDialog(eventID, eventID);
                if (initialDialog == null) Debug.Log($"*{this.GetType().Name} - Failed to Load DialogData");
                break;

            case EventType.Narration:
                narrations = database.GetNarration(eventID);
                if (narrations == null) Debug.Log($"*{this.GetType().Name} - Failed to Load NarrationData");
                break;
        }
    }

    protected virtual void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(PlayEvent);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(EndChoice);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(CloseDialog);
    }
    protected virtual void  OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(PlayEvent);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(EndChoice);
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(CloseDialog);
    }

    protected virtual void PlayEvent(GameEvents.PlayEvent evt) { 
        
    }

    protected virtual void EndChoice(UIEvents.MakeChoice evt) {
        
        // Event선택지일때만
        if (!evt.isNpc) {
            // choiceData에서 continue 꺼냄
            string continueID = null;
            if (evt.data != null && evt.data.continueIDs.Count > evt.selectIdx) {
                continueID = evt.data.continueIDs[evt.selectIdx];
            }

            if (!string.IsNullOrEmpty(continueID))
            {
                if (allDialogs.TryGetValue(continueID, out var branchDialog))
                {
                    // 선택지 이후 분기대화
                    EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, branchDialog));
                }
                else
                {
                    Debug.Log($"{continueID}에 대한 분기대화 없음");
                }
            }
            else {
                // 선택지는 있으나, 그 후 이벤트 없을경우
                EventBus.Instance.Publish<UIEvents.CloseDialog>(new UIEvents.CloseDialog(false));
            }
        }
    }


    protected virtual void CloseDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == eventID) {
            Debug.Log("이벤트 로그 후 반드시 Inspector모드로 고정시키는 곳......");
            // 게임모드 변경
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }
    }

}
