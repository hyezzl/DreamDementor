using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SorrowIntro : EventBase
{
    private static bool SorrowSceneFirstVisit = false;

    public override void Init(IDatabase db)  // start시점에 실행
    {
        base.Init(db);
        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        if (SorrowSceneFirstVisit)
        {
            // 브금 재생
            EventBus.Instance.Publish<GameEvents.PlayBGM>(new GameEvents.PlayBGM(BGMType.HappyBGM));

            // 빗소리
            EventBus.Instance.Publish<GameEvents.PlayLoopSFX>(new GameEvents.PlayLoopSFX(SFXType.rainSound));

            SorrowStart();
        }
        else {
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }

    }

    private void SorrowStart() {
        // 슬픔맵 첫 입장 시 등장

        // 이벤트 시작 (대화)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));

        // 대화모드
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
    }
}
