using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappyOneFirst : SceneStart
{
    [SerializeField] private GameObject happyOneTuto;
    [SerializeField] private GameObject miniQuest;

    [SerializeField] private Button yesBTN;
    private bool clickYes = false;


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

        // 확인버튼 눌릴 때 까지 대기
        yield return WaitForYesButton();



        // 시간 재개
        EventBus.Instance.Publish<GameEvents.FlowTime>(new GameEvents.FlowTime(false));

        happyOneTuto.SetActive(false);

        // 퀘스트 생성
        miniQuest.SetActive(true);
    }

    // 버튼
    private IEnumerator WaitForYesButton()
    {
        clickYes = false;
        yesBTN.onClick.AddListener(OnClickYes);

        yield return new WaitUntil(() => clickYes);

        yesBTN.onClick.RemoveListener(OnClickYes);
    }

    private void OnClickYes()
    {
        clickYes = true;
    }


}
