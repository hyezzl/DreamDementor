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

public class IntroEvent : EventBase
{
    public string illID = "F001";

    [Header("UI Refs")]
    [SerializeField] private Image background_white;    // 하얀배경
    [SerializeField] private Image background;      // 검은배경
    [SerializeField] private CanvasGroup backgroundGroup;
    [SerializeField] private CanvasGroup background_black;      // 검은배경 캔버스그룹
    [SerializeField] private Image illust;          // 일러스트
    [SerializeField] private CanvasGroup illustGroup;
    [SerializeField] private CanvasGroup book;
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private CanvasGroup arrow;
    
    private FullIllustration fullIll;

    private BlinkAnnounce blink;
    private bool standbyInput = false;  // 사용자의 입력 기다림 상태
    private Coroutine blinkCor;

    // 타이핑 변수
    [SerializeField] private float typingSpeed = 0.05f;
    Tweener typing;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<BlinkAnnounce>(out blink)) Debug.Log("IntroEvent - Failed to Load BlinkAnnounce");
        fullIll = FindAnyObjectByType<FullIllustration>();
        if (fullIll == null) Debug.Log("IntroEvent - Failed to Load FullIllustation");
    }

    // DB 연결
    public override void Init(IDatabase db)  // start시점에 실행
    { 
        base.Init(db);

        StartCoroutine(PlayIntro());
    }

    public IEnumerator PlayIntro() {
        // 이벤트 시작
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));

        // 게임모드
        pc.CurMode = GameMode.EventMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

        // 일러스트 3초간 전시
        EventBus.Instance.Publish<UIEvents.SceneCover>(new UIEvents.SceneCover(illID));
        yield return new WaitForSeconds(3f);

        // 하얀배경 끄기
        background_white.gameObject.SetActive(false);
        background.gameObject.SetActive(true);

        // 2. 일러스트 이미지 확대
        Vector2 zoomPivot = new Vector2(0.6265317f, 0.320314f);
        Vector3 zoomPosition = Vector3.zero;
        float zoomScale = 2f;
        float zoomDuration = 4f;

        yield return StartCoroutine(fullIll.ZoomInAtPoint(
            illust.rectTransform,
            zoomPivot,
            zoomPosition,
            zoomScale,
            zoomDuration));

        // 책 등장
        //book.alpha = 1f;
        //yield return StartCoroutine(FadeGroup(book, 1f, 0f, 1f));
        // 과 동시에 페이드 아웃
        //yield return StartCoroutine(FadeGroup(book, 1f, 0f, 0.15f));
        //StartCoroutine(FadeGroup(book, 1.5f, 1f, 0.1f));


        // 3. 검은 배경과 텍스트 활성화
        //book.alpha = 0.1f;
        //background.gameObject.SetActive(true);
        textArea.gameObject.SetActive(true);
        textArea.text = "";

        // 일러스트 끔
        EventBus.Instance.Publish<UIEvents.SceneDiscover>(new UIEvents.SceneDiscover());

        // 4. 배경 페이드인
        //yield return StartCoroutine(FadeInBackground(1f, true));
        yield return StartCoroutine(FadeGroup(book, 1.5f, 0f, 0.04f));

        // 5. 나레이션 시작 및 완료 대기
        yield return StartCoroutine(PlayNarration());

        // 6. 완료 이후 
        if (blinkCor != null) { 
            StopCoroutine(blinkCor);
        }
        textArea.gameObject.SetActive(false);
        StartCoroutine(FadeInBackground(0.7f, false));
        
        // 타이핑 끝나고 화면 꺼주기
        //background.gameObject.SetActive(false);

        // 바로 다음 이벤트 호출
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent("E003"));
    }


    private IEnumerator PlayNarration() {
        yield return null;

        if (narrations == null) {
            Debug.Log("IntroEvent - Failed to Load NarrationList");
            yield break;
        }

        blinkCor = StartCoroutine(BlinkArrow()); // Arrow blink 0.5초후 시작

        foreach (var narration in narrations.OrderBy(n => n.order)) {
            float duration = narration.text.Length * typingSpeed;
            var typing = textArea.DOText(narration.text + "\n", duration).SetEase(Ease.Linear);

            arrow.alpha = 0f;

            // 타이핑 완료까지 대기
            yield return typing.WaitForCompletion();
            standbyInput = true;
            //blinkCor = StartCoroutine(BlinkArrow()); // Arrow blink 0.5초후 시작

            // 사용자의 입력 대기 (스페이스)
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            yield return null;

            arrow.alpha = 0f;
            standbyInput = false;
            textArea.text = ""; //초기화
        }
    }

    private IEnumerator BlinkArrow() {
        yield return null;
        //if (standbyInput) {
            StartCoroutine(blink.BlinkAnnounceMSG(arrow, 1.2f));
        //}
    }

    public void RecordEvent(GameEvents.EndEvent evt) { } // 잠만

    private IEnumerator DelayAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (narrations != null)
        {
            StartCoroutine(PlayNarration());
        }
    }

    private IEnumerator FadeInBackground(float duration, bool isIn)
    {
        float elapsed = 0f;
        //backgroundGroup.gameObject.SetActive(true);

        if (isIn)
        {
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                backgroundGroup.alpha = Mathf.Clamp01(elapsed / duration);
                background_black.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            backgroundGroup.alpha = 1f;
        }
        else {
            illustGroup.alpha = 0f;
            book.alpha = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                //illustGroup.alpha = 1 - Mathf.Clamp01(elapsed / duration);
                backgroundGroup.alpha = 1 - Mathf.Clamp01(elapsed / duration);
                background_black.alpha = 1 - Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            backgroundGroup.alpha = 0f;
            background_black.alpha = 0f;
        }
    }

    private IEnumerator FadeGroup(CanvasGroup group, float duration, float startVal, float endVal) {
        float elapsed = 0f;

        while (elapsed < duration) { 
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startVal, endVal, elapsed / duration);
            yield return null;
        }
        group.alpha = endVal;
    }

}
