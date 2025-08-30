using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 게임시작하자마자 실행될 Intro Event + 다음 이벤트 이어 호출
/// </summary>

public class IntroEvent : MonoBehaviour, IGameEvent
{
    public string eventID = "E001";
    private IDatabase database;

    [Header("UI Refs")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private CanvasGroup arrow;
    private List<NarrationData> narrations;

    private BlinkAnnounce blink;
    private PlayerController pc;
    private bool standbyInput = false;  // 사용자의 입력 기다림 상태
    private Coroutine blinkCor;

    // 타이핑 변수
    [SerializeField] private float typingSpeed = 0.05f;
    Tweener typing;

    private void Awake()
    {
        if (!TryGetComponent<BlinkAnnounce>(out blink)) Debug.Log("IntroEvent - Failed to Load BlinkAnnounce");
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("IntroEvent - Failed to Load PlayerController");
    }


    // DB 연결
    public void Init(IDatabase db)  // start시점에 실행
    { 
        database = db;
        narrations = database.GetNarration(eventID);
        if (narrations == null) Debug.Log("IntroEvent - Failed to Load NarrationData");

        PlayIntro();
    }


    public void PlayIntro() {
        // 이벤트 시작
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));

        // 게임모드
        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 검은 바탕 활성화
        background.gameObject.SetActive(true);
        textArea.gameObject.SetActive(true);
        textArea.text = "";

        // 타이핑 시작
        StartCoroutine(PlayNarration());
    }


    private IEnumerator PlayNarration() {
        yield return null;

        if (narrations == null) {
            Debug.Log("IntroEvent - Failed to Load NarrationList");
            yield break;
        }

        foreach (var narration in narrations.OrderBy(n => n.order)) {
            float duration = narration.text.Length * typingSpeed;
            var typing = textArea.DOText(narration.text + "\n", duration).SetEase(Ease.Linear);

            arrow.alpha = 0f;

            // 타이핑 완료까지 대기
            yield return typing.WaitForCompletion();
            standbyInput = true;
            blinkCor = StartCoroutine(BlinkArrow()); // Arrow blink 0.5초후 시작

            // 사용자의 입력 대기 (스페이스)
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return null;

            arrow.alpha = 0f;
            standbyInput = false;
            textArea.text = ""; //초기화
        }

        // 타이핑 끝나고 화면 꺼주기
        StopCoroutine(blinkCor);
        background.gameObject.SetActive(false);
        textArea.gameObject.SetActive(false);

        // 바로 다음 이벤트 호출
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent("E003"));
    }

    private IEnumerator BlinkArrow() {
        yield return null;
        if (standbyInput) {
            StartCoroutine(blink.BlinkAnnounceMSG(arrow, 1.2f));
        }
    }

    public void RecordEvent(GameEvents.PlayEvent evt)
    {
        // 저장
    }
}
