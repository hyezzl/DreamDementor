using System.Collections;
using System.Collections.Generic;
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
        EventBus.Instance.Subscribe<GameEvents.MakeChoice>(EndChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(PlayTutorial);
        EventBus.Instance.Unsubscribe<GameEvents.MakeChoice>(EndChoice);
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
        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.BetweenCam));
        // Dialog박스 비활성화
        dialog.alpha = 0f;


        // 2. Dialog + 선택지
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(dialogs));

        // 대화 이벤트 중 캐릭터들 애니메이션
        playerAnim.Play("PlayerIdle_Right", 0);
        //EnemyAnim.Play("MomIdle_Left");


        // 선택까지 대기
        yield return new WaitUntil(() => isChoice == true);

        // 3. 두번째 타임라인 재생
        Debug.Log("들어오나???");
        //timeline02.Play();
        //yield return new WaitUntil(() => timeline02.state != PlayState.Playing);
        //
        //// 술래잡기 시작
        //Debug.Log("술래잡기 시작~~~~~~~~~");
        //// 게임모드 변경
        //pc.CurMode = GameMode.InspectMode;
        //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        //EventBus.Instance.Publish<GameEvents.EndTimeline>(new GameEvents.EndTimeline());
        //
        //// 적 상태 변경 (Chase)
        //EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
    }

    public void EndChoice(GameEvents.MakeChoice evt) {
        Debug.Log("MakeChoice 이벤트 받음!");
        isChoice = true;
    }

}
