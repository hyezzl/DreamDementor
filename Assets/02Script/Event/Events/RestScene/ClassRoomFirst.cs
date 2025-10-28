using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClassRoomFirst : SceneStart
{
    public override void Init(IDatabase db)
    {
        base.Init(db);

        // 시작 시 3인칭 모드
        PlayerController.Instance.CurAspect = AspectMode.ThirdpersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.ThirdpersonMode));

    }

    protected override void OnFirstVisit()
    {
        base.OnFirstVisit();

        // 플레이어 방향 강제
        EventBus.Instance.Publish(new GameEvents.ForceDir(Direction4.Right));

        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }

}
