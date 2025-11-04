using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEvents;


/// <summary>
/// 씬의 시작 클래스
/// </summary>

public class SceneStart : MonoBehaviour, IGameEvent
{
    [Header("Scene Info")]
    public SceneType curScene;

    [Header("Start Event")]
    public string startEventID;
    public EventType eventtype;

    protected IDatabase database;
    protected PlayerController pc;
    protected Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    protected Dictionary<int, DialogData> initialDialog;
    protected List<NarrationData> narrations;
    protected bool isFirst = true;  // 첫방문인지

    protected virtual void Awake() { }


    protected virtual void OnEnable() 
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(AfterDialog);
        EventBus.Instance.Subscribe<GameEvents.SceneEffectEnd>(EndSceneEffect);
    }
    protected virtual void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(AfterDialog);
        EventBus.Instance.Unsubscribe<GameEvents.SceneEffectEnd>(EndSceneEffect);
    }


    public virtual void Init(IDatabase db)
    {
        database = db;
        Debug.Log($"*{this.GetType().Name} 의 Init 완료!");

        switch (eventtype)
        {
            case EventType.Conversation:
                allDialogs = database.GetDialogEvent(startEventID);
                initialDialog = database.GetDialog(startEventID, startEventID);
                if (initialDialog == null) Debug.Log($"*{this.GetType().Name} - Failed to Load DialogData");
                break;

            case EventType.Narration:
                narrations = database.GetNarration(startEventID);
                if (narrations == null) Debug.Log($"*{this.GetType().Name} - Failed to Load NarrationData");
                break;
        }
        isFirst = EventHistoryManager.Instance.IsFirstVisit(curScene);


        // 씬 재방문 조사
        if (EventHistoryManager.Instance.IsFirstVisit(curScene))
        {
            // 첫방문이면

            // 씬 전환 세이브가 선행
            SaveManager.Instance.ManualSave();
            SaveManager.Instance.SaveGame();

            // 씬 방문 정보 갱신
            EventHistoryManager.Instance.RecordVisit(curScene);
            
            // 첫 방문 시 실행될 함수
            OnFirstVisit();
        }
        else {
            // 재방문이면

            // 씬 전환 세이브가 선행
            SaveManager.Instance.ManualSave();
            SaveManager.Instance.SaveGame();

            // 재 방문 실행 함수
            OnRevisit();
        }
    }

    /// <summary>
    /// 첫방문일 때 실행될 함수 (직후)
    /// </summary>
    protected virtual void OnFirstVisit() {
        Debug.Log($"{curScene} : 첫방문입니다.");
        // 씬여는 효과 생략? or not? (base)
        EventBus.Instance.Publish<GameEvents.SceneStartEffect>(new GameEvents.SceneStartEffect(curScene));
    }

    /// <summary>
    /// 재방문일 때 실행될 함수 (직후)
    /// </summary>
    protected virtual void OnRevisit() {
        Debug.Log($"{curScene} : 재방문입니다.");
        // 씬여는 효과
        EventBus.Instance.Publish<GameEvents.SceneStartEffect>(new GameEvents.SceneStartEffect(curScene));
    }

    /// <summary>
    /// 씬 첫방문 + 열리는 연출 직후 실행될 함수
    /// </summary>
    protected virtual void OnSceneStart() { }


    /// <summary>
    /// 씬 재방문 + 열리는 연출 직후 실행될 함수
    /// </summary>
    protected virtual void OnSceneRestart() { }




    // 씬전환 이벤트 끝나고
    protected void EndSceneEffect(GameEvents.SceneEffectEnd evt) {
        if (SwitchSceneManager.Instance.CurScene == evt.scene) {
            if (isFirst)
            {
                // 첫방문 + 씬오픈효과 직후
                OnSceneStart();
                isFirst = false;
            }
            else {
                // 재방문 + 씬오픈효과 직후
                OnSceneRestart();
            }
        }
    }

    // 첫 이벤트 끝나고 난 후
    protected virtual void AfterDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == startEventID) 
        {
            // 첫 대화 이벤트 끝난 후
            isFirst = false;
        }
    }
}
