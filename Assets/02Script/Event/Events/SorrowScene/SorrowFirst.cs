using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowFirst : SceneStart
{
    [Header("ReVisit")]
    public string reEventID;        // 재방문 시 실행될 이벤트
    protected Dictionary<int, DialogData> revisitDialog;

    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.SorrowScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    {
        base.Init(db);

        if (!isFirst)
        {
            // 재방문일 때만 전용 다이얼로그 로드
            if (!string.IsNullOrEmpty(reEventID))
            {
                revisitDialog = database.GetDialog(reEventID, reEventID);
                if (revisitDialog == null) Debug.Log("SorrowFirst - Failed to Load ReVisitEventData");
            }
        }

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));
    }


    // 1. 첫방문 로직
    protected override void OnFirstVisit()
    {
        Debug.Log("슬픔맵 첫방문!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        // 스테이지 첫 시작
        EventBus.Instance.Publish<GameEvents.NewStageStart>(new GameEvents.NewStageStart(Stage.Sorrow));
    
        base.OnFirstVisit();    // 씬여는 효과
    }


    protected override void OnSceneStart()
    {
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }


    // 2. 재방문 로직
    protected override void OnRevisit()
    {
        base.OnRevisit(); // SceneStartEffect 발행
        Debug.Log("슬픔맵 재방문");

        // 재방문 시 필요한 자동 로드 등 처리
        SaveManager.Instance.AutoLoad();
    }

    protected override void OnSceneRestart()
    {
        base.OnSceneRestart();

        if (revisitDialog != null)
        {
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

            // 재방문 이벤트 발행
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(reEventID));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(reEventID, revisitDialog, GameMode.InspectMode));
        }
        else
        {
            PlayerController.Instance.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }
    }
}
