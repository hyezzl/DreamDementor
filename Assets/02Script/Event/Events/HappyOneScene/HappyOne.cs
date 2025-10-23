using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HappyOne : EventBase
{
    protected void Start()
    {
        // 시작 시 1인칭 모드
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

        // 게임모드 InspectorMode
        pc.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }


    public override void Init(IDatabase db)  // start시점에 실행
    {
        base.Init(db);

        StartCoroutine(PlayIntro());
    }


    private IEnumerator PlayIntro() {
        yield return null;

        // 맵 입장하자마자 이벤트 (대화 이벤트)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
    }

}
