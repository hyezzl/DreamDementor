using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialEvent : EventBase
{
    private PlayerMove pm;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector timeline01;
    [SerializeField] private PlayableDirector timeline02;

    [Header("UIRefs")]
    [SerializeField] private CanvasGroup dialog;
    [SerializeField] private GameObject tutorialPopup;

    [Header("Animator")]
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Animator EnemyAnim;

    [Header("Filter Refs")]
    [SerializeField] private CameraFilterPack_FX_Glitch1 glitch;

    private bool isChoice = false;


    protected override void Awake()
    {
        base.Awake();
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("TutorialEvent - Failed to Load PlayerMove");

        glitch = Camera.main.GetComponent<CameraFilterPack_FX_Glitch1>();
        if (glitch == null) Debug.Log("TutorialEvent - Failed to Load GlitchFilter");
    }

    protected override void PlayEvent(GameEvents.PlayEvent evt) {
        if (evt.eventID != eventID) return;

        // 타임라인
        StartCoroutine(PlayTimeline());
        
    }

    // 타임라인 실행
    private IEnumerator PlayTimeline() {
        // 카메라 필터
        EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.OldMovie, true));

        // 1. 타임라인01 재생
        timeline01.Play();

        // 재생완료까지 대기
        yield return new WaitUntil(() => timeline01.state != PlayState.Playing);
        // 카메라 BetweenCam으로 고정
        EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.BetweenCam, 0));

        // 2. Dialog + 선택지
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));

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

        // 카메라 필터
        EventBus.Instance.Publish<GameEvents.FilterOn>(new GameEvents.FilterOn(FilterType.Glitch, true));
        if(glitch != null)
            glitch.Glitch = 0.4f;

        yield return null;


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

        // 필터 강도 변경
        if(glitch != null)
            glitch.Glitch = 0.1f;

        pc.CurMode = GameMode.EventInInspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventInInspectMode));


        //EventBus.Instance.Publish<GameEvents.EndTimeline>(new GameEvents.EndTimeline());
        
        // 적 상태 변경 (Chase)
        EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
    }

    protected override void EndChoice(UIEvents.MakeChoice evt) {

        //if(evt.choiceID == ) 다음 이벤트 부터 처리
        if (evt.selectIdx == 0 || evt.selectIdx == 1) { 
            isChoice = true;
        }
    }

}
