using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowOneFirst : SceneStart
{
    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.SorrowOneScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    {
        base.Init(db);

        // 기존 효과음 삭제
        EventBus.Instance.Publish<GameEvents.StopSFX>(new GameEvents.StopSFX());

        // 시작 시 1인칭 모드
        PlayerController.Instance.CurAspect = AspectMode.OnepersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        SaveManager.Instance.SaveGame();
    }

    protected override void OnSceneStart()
    {
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }

    protected override void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            // 첫 대화 이벤트 끝난 후
            // 미니퀘스트 활성화
        }
    }
}
