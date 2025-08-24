using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialEvent : MonoBehaviour
{
    public string eventID = "E002";
    private IDatabase database;
    private List<CutsceneData> cutsceneTexts;

    private PlayerMove pm;
    private PlayerController pc;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector timeline01;
    [SerializeField] private PlayableDirector timeline02;
    [SerializeField] private Canvas speech;

    private bool isChoice = false;

    [Header("임시")]
    [SerializeField] public List<string> texts = new();



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
        cutsceneTexts = database.GetCutscene(eventID);
        if (cutsceneTexts == null) Debug.Log("TutorialEvent - Failed to Load CutsceneData");
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(PlayTutorial);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(PlayTutorial);
    }

    private void PlayTutorial(GameEvents.PlayEvent evt) {
        if (evt.eventID != eventID) return;

        // 타임라인
        StartCoroutine(PlayTimeline());
        
    }

    // 타임라인 실행
    private IEnumerator PlayTimeline() {
        timeline01.Play();

        // 재생완료까지 대기
        yield return new WaitUntil(() => timeline01.state != PlayState.Playing);
        speech.gameObject.SetActive(false);


        // Dialog + 선택지
        // 게임모드 변경
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(texts));



        // 선택까지 대기
        yield return new WaitUntil(() => isChoice == true);

        // 두번째 타임라인 재생 + 완료대기
        Debug.Log("왜안들어와요?????????????????????/////");
        timeline02.Play();
        yield return new WaitUntil(() => timeline02.state != PlayState.Playing);

        // 술래잡기 시작
        Debug.Log("술래잡기 시작~~~~~~~~~");
        // 게임모드 변경
        pc.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        EventBus.Instance.Publish<GameEvents.EndTimeline>(new GameEvents.EndTimeline());

        // 적 상태 변경 (Chase)
        EventBus.Instance.Publish<GameEvents.EnemyStateChange>(new GameEvents.EnemyStateChange(EnemyState.Chase));
    }

    // 외부에서 호출할 함수
    public void OnChoice(int idx) {
        isChoice = true;
    }

}
