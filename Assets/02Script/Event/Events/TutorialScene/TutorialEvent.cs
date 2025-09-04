using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialEvent : MonoBehaviour
{
    public string eventID = "E003";
    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;

    private PlayerMove pm;
    private PlayerController pc;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector timeline01;
    [SerializeField] private PlayableDirector timeline02;

    [Header("UIRefs")]
    [SerializeField] private CanvasGroup dialog;
    [SerializeField] private GameObject tutorialPopup;

    [Header("Animator")]
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Animator EnemyAnim;

    private bool isChoice = false;


    private void Awake()
    {
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("TutorialEvent - Failed to Load PlayerMove");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PlayerController - Failed to Load PlayerController");
    }

    // DB 연결
    public void Init(IDatabase db) {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialEvent - Failed to Load DialogData");
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(PlayTutorial);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(EndChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(PlayTutorial);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(EndChoice);
    }

    private void PlayTutorial(GameEvents.PlayEvent evt) {
        if (evt.eventID != eventID) return;

        // 타임라인
        StartCoroutine(PlayTimeline());
        
    }

    // 타임라인 실행
    private IEnumerator PlayTimeline() {
        // 1. 타임라인01 재생
        timeline01.Play();

        // 재생완료까지 대기
        yield return new WaitUntil(() => timeline01.state != PlayState.Playing);
        // 카메라 BetweenCam으로 고정
        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.BetweenCam, 0));

        // 2. Dialog + 선택지
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));

        // 대화 이벤트 중 캐릭터들 애니메이션 
        playerAnim.SetFloat("lookX", 1f);
        playerAnim.SetFloat("lookY", 0f);
        

        // 을 멈추고 싶다고 하십니다. 리더께서.
        playerAnim.speed = 0f;
        EnemyAnim.speed = 0f;


        // 선택까지 대기
        yield return new WaitUntil(() => isChoice == true); // 이벤트 발생

        

        // 3. 두번째 타임라인 재생
        timeline02.Play();
        yield return new WaitUntil(() => timeline02.state != PlayState.Playing);

        yield return null;


        // 튜토리얼(Pause) 모드
        //pc.CurMode = GameMode.PauseMode;
        //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
        //EndTimeline Event Publish 필요한가?

        // 카메라 고정
        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.PlayerFixCam,0));
    


        // 애니메이션 다시 재생
        playerAnim.speed = 1f;
        EnemyAnim.speed = 1f;


        // 애니메이션 고정
        playerAnim.SetFloat("lookX", 1f);
        playerAnim.SetFloat("lookY", 0f);
        EnemyAnim.Play("Temp01", 0);

        // 시간 멈춤
        EventBus.Instance.Publish<GameEvents.StopTime>(new GameEvents.StopTime(false));

        // 튜토리얼 화면 나오게
        tutorialPopup.SetActive(true);

        yield return new WaitUntil(() => Input.anyKeyDown);

        

        // 시간 재개
        EventBus.Instance.Publish<GameEvents.FlowTime>(new GameEvents.FlowTime(false));

        // 술래 잡기 시작
        tutorialPopup.SetActive(false);
        Debug.Log("술래잡기 시작~~~~~~~~~");

        pc.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

        

        //EventBus.Instance.Publish<GameEvents.EndTimeline>(new GameEvents.EndTimeline());
        
        // 적 상태 변경 (Chase)
        //EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
    }

    public void EndChoice(UIEvents.MakeChoice evt) {

        //if(evt.choiceID == ) 다음 이벤트 부터 처리
        if (evt.selectIdx == 0 || evt.selectIdx == 1) { 
            isChoice = true;
        }
    }

}
