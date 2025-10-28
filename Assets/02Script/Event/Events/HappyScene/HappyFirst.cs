using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class HappyFirst : SceneStart
{
    [SerializeField] PlayableDirector eyesOpen;
    [SerializeField] private Canvas eyeCanvas;
    [SerializeField] private PostProcessVolume pp;
    [SerializeField] private GameObject happyTutorial;

    private int[] requireIDs = { 10001002, 10001003, 10001004 };


    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.HappyScene;
        base.Awake();
    }


    protected override void OnFirstVisit() {
        // 씬여는 효과 생략하고 개인 이벤트시작
        StartCoroutine(PlayEvents());
    }

    protected override void OnRevisit()
    {
        base.OnRevisit();
        pp.gameObject.SetActive(false);


        // 재방문 + 열쇠 3개 모두 가지고있을경우   
        if (IsSatisfying()) {
            // 19번 이벤트 발행
            //EventBus.Instance.Publish<GameEvents.PlayEvent>();
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog("E019", initialDialog));
        }
    }

    protected override void OnSceneStart()
    {

    }


    private IEnumerator PlayEvents()
    {
        PlayerController.Instance.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 눈열리는 연출
        eyesOpen.Play();
        yield return new WaitUntil(() => eyesOpen.state != PlayState.Playing);

        // 이벤트 시작 (대화)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(startEventID));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
    }


    // 열쇠 3개가 모두 있는지 
    private bool IsSatisfying()
    {
        // 인벤토리 가져옴
        var inventory = InventoryManager.Instance.GetInventory();

        foreach (var id in requireIDs)
        {
            bool hasKey = false;
            foreach (var item in inventory)
            {
                if (item.itemID == id)
                {
                    hasKey = true;
                    break;
                }
            }
            if (!hasKey)
                return false;   // 하나라도 없으면 false
        }
        return true;
    }

    protected override void AfterDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == startEventID)
        {
            // 첫 대화 이벤트 끝난 후
            StartCoroutine(PlayTuto());
        }
    }

    private IEnumerator PlayTuto() {
        // 시간 멈춤
        EventBus.Instance.Publish<GameEvents.StopTime>(new GameEvents.StopTime(false));

        // 튜토리얼 화면 나오게
        happyTutorial.SetActive(true);

        yield return new WaitUntil(() => Input.anyKeyDown);


        // 시간 재개
        EventBus.Instance.Publish<GameEvents.FlowTime>(new GameEvents.FlowTime(false));

        happyTutorial.SetActive(false);
    }

}
