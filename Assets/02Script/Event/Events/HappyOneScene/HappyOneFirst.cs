using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyOneFirst : SceneStart
{
    [SerializeField] private GameObject happyOneTuto;


    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.HappyOneScene;
        base.Awake();
    }

    public override void Init(IDatabase db)
    { 
        base.Init(db);

        // 시작 시 1인칭 모드
        PlayerController.Instance.CurAspect = AspectMode.OnepersonMode;
        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));

        // Inspector모드
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));
    }

    protected override void OnFirstVisit() { 
        base.OnFirstVisit();
    }

    protected override void OnSceneStart()
    {

        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }

    private IEnumerator PlayIntro()
    {
        yield return null;

        // 맵 입장하자마자 이벤트 (대화 이벤트)
        //EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        //EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }

    protected override void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            // 첫 대화 이벤트 끝난 후
            StartCoroutine(PlayTuto());
        }
    }

    private IEnumerator PlayTuto()
    {
        // 시간 멈춤
        EventBus.Instance.Publish<GameEvents.StopTime>(new GameEvents.StopTime(false));

        // 튜토리얼 화면 나오게
        happyOneTuto.SetActive(true);

        yield return new WaitUntil(() => Input.anyKeyDown);



        // 시간 재개
        EventBus.Instance.Publish<GameEvents.FlowTime>(new GameEvents.FlowTime(false));

        happyOneTuto.SetActive(false);
    }


}
