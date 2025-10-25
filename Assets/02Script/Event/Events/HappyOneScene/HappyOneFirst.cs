using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyOneFirst : SceneStart
{
    public override void Init(IDatabase db)
    { 
        base.Init(db);

        // 시작 시 1인칭 모드
        pc.CurAspect = AspectMode.OnepersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

    }

    protected override void OnFirstVisit() { 
        base.OnFirstVisit();

        StartCoroutine(PlayIntro());
    }



    private IEnumerator PlayIntro()
    {
        yield return null;

        // 맵 입장하자마자 이벤트 (대화 이벤트)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }


}
