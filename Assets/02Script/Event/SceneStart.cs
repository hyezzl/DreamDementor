using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    protected virtual void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log($"*{this.GetType().Name} - Failed to Load PlayerController");
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

        // PlayerPrefs사용한 첫방문 분기
        //bool happyFirstVisit = PlayerPrefs.GetInt("HappySceneFirstVisit", 0) == 0;  // 첫방문인가?

        //if (happyFirstVisit) { 
        //    eyeCanvas.gameObject.SetActive(true);
        //    PlayTeleport();
        //    PlayerPrefs.SetInt("HappySceneFirstVisit", 1);
        //    PlayerPrefs.Save();

        //    //브금 재생
        //}

        // 씬 재방문 조사
        if (EventHistoryManager.Instance.isFirstVisit(curScene))
        {
            // 첫방문이면
            OnFirstVisit();

            // 브금재생
        }
        else {
            // 재방문이면
            OnRevisit();
        }
    }

    /// <summary>
    /// 첫방문일 때 실행될 함수
    /// </summary>
    protected virtual void OnFirstVisit() {
        Debug.Log($"{curScene} : 첫방문입니다.");
        EventHistoryManager.Instance.RecordVisit(curScene);
    }

    /// <summary>
    /// 재방문일 때 실행될 함수
    /// </summary>
    protected virtual void OnRevisit() {
        Debug.Log($"{curScene} : 재방문입니다.");
    }

}
