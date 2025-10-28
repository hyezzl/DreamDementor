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

        // PlayerPrefs사용한 첫방문 분기
        //bool sorrowFirstVisit = PlayerPrefs.GetInt("SorrowSceneFirstVisit", 0) == 0;  // 첫방문인가?

        //if (sorrowFirstVisit) { 
        //    PlayerPrefs.SetInt("SorrowSceneFirstVisit", 1);
        //    PlayerPrefs.Save();

        //브금 재생
        //EventBus.Instance.Publish<GameEvents.PlayBGM>(new GameEvents.PlayBGM(BGMType.HappyBGM));
    }

    protected override void OnSceneStart()
    {
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }


}
