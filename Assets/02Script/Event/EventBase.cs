using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    Narration,
    Cutscene,
    Conversation,  // only 대화 (플레이어의 입력 포함)
    SceneChange,
}

public class EventBase : MonoBehaviour, IGameEvent
{
    [Header("EventInfo")]
    public string eventID;
    public EventType eventtype;

    protected IDatabase database;
    protected Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    protected Dictionary<int, DialogData> initialDialog;
    protected List<NarrationData> narrations;

    // DB연결
    public virtual void Init(IDatabase db) {
        database = db;
        Debug.Log($"{this.GetType().Name} 의 Init 완료!");

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
        //allDialogs = database.GetDialogEvent(eventID);
        //initialDialog = database.GetDialog(eventID, eventID);
        //if (initialDialog == null) Debug.Log($"{this.GetType().Name} - Failed to Load Dialog");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(PlayEvent);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(EndChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(PlayEvent);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(EndChoice);
    }

    protected virtual void PlayEvent(GameEvents.PlayEvent evt) { 
        
    }

    protected virtual void EndChoice(UIEvents.MakeChoice evt) { 
        
    }

}
