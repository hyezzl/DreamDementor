using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowFirst : SceneStart
{
    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.SorrowScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    {
        base.Init(db);

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));
    }

    protected override void OnFirstVisit()
    {
        Debug.Log("슬픔맵 첫방문!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        // 스테이지 첫 시작
        EventBus.Instance.Publish<GameEvents.NewStageStart>(new GameEvents.NewStageStart(Stage.Sorrow));
    
        base.OnFirstVisit();
    }


    protected override void OnSceneStart()
    {
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }
}
