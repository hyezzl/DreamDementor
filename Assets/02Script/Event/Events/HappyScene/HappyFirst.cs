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

    // 재방문
    public string reEventID;
    protected Dictionary<int, DialogData> revisitDialog;



    private int[] requireIDs = { 10001002, 10001003, 10001004 };


    protected override void Awake()
    {
        SwitchSceneManager.Instance.CurScene = SceneType.HappyScene;
        base.Awake();
    }

    public override void Init(IDatabase db) { 
        base.Init(db);
        Debug.Log("여기뜨나욤???????????????");
        revisitDialog = database.GetDialog(reEventID, reEventID);
        if (revisitDialog == null) Debug.Log($"*{this.GetType().Name} - Failed to Load DialogData");
    }



    protected override void OnFirstVisit() {
        // 첫 방문
        Debug.Log("해피맵 첫방문!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        // 스테이지 첫 시작
        EventBus.Instance.Publish<GameEvents.NewStageStart>(new GameEvents.NewStageStart(Stage.Happy));

        // 씬여는 효과 생략하고 개인 이벤트시작
        StartCoroutine(PlayEvents());

    }

    protected override void OnRevisit()
    {
        base.OnRevisit();
        Debug.Log("해피맵 재방문!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");


        pp.gameObject.SetActive(false);

        if (revisitDialog == null) {
            Debug.Log("들어오지맛!!!");
            revisitDialog = database.GetDialog(reEventID, reEventID);
        }

        // 재방문 + 열쇠 3개 모두 가지고있을경우   
        if (IsSatisfying()) {
            // 19번 이벤트 발행
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(reEventID, revisitDialog, GameMode.InspectMode));
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
        //EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.InspectMode));
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(startEventID, initialDialog, GameMode.EventMode));
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

        // 강제 이벤트모드
        //PlayerController.Instance.CurMode = GameMode.EventMode;
        //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 튜토리얼 화면 나오게
        happyTutorial.SetActive(true);

        // 확인 버튼 눌릴 때 까지 대기
        yield return WaitForSpace();

        happyTutorial.SetActive(false);

        PlayerController.Instance.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
    }


    private IEnumerator WaitForSpace()
    {
        // 3.5초 동안 스페이스 입력 무시 (딜레이)
        float delayTime = 3.3f;
        float timer = 0f;

        while (timer < delayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 딜레이 후부터 스페이스 입력 대기
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
    }

}
