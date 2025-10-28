using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class HappyFirst : SceneStart
{
    [SerializeField] PlayableDirector eyesOpen;
    [SerializeField] private Canvas eyeCanvas;
    [SerializeField] private PostProcessVolume pp;

    // 튜토리얼
    [SerializeField] private GameObject happyTutorial;
    [SerializeField] private Button yesBTN;

    private bool isClickY = false;      // 확인 버튼 눌렸는가?


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

        // 마우스 임시 활성화 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;


        // 튜토리얼 화면 나오게
        happyTutorial.SetActive(true);

        // 확인 버튼 눌릴 때 까지 대기
        yield return WaitForYesButton();

        // 마우스 임시 비활성화 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        // 시간 재개
        EventBus.Instance.Publish<GameEvents.FlowTime>(new GameEvents.FlowTime(false));

        happyTutorial.SetActive(false);
    }

    private IEnumerator WaitForYesButton()
    {
        isClickY = false;
        yesBTN.onClick.AddListener(OnYesClicked);  // 리스너 등록

        yield return new WaitUntil(() => isClickY);

        yesBTN.onClick.RemoveListener(OnYesClicked);  // 리스너 해제
    }

    private void OnYesClicked()
    {
        isClickY = true;
    }

}
