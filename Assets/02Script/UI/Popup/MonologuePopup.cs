using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class MonologuePopup : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 20f;

    [Header("UI Refs")]
    [SerializeField] private CanvasGroup monoBox;
    [SerializeField] private TextMeshProUGUI monoText;

    private PlayerController pc;
    Tweener typing;
    Sequence seq;
    private bool isOpen = false;
    private bool isTyping = false;
    private bool standbyInput = false; // 사용자 입력 기다리기
    private bool isSkip = false;

    private GameMode preMode;       // 캐싱
    private IInputHandler inputHandler;
    private string sentence;        // 캐싱
    private string curEventID;  // 캐싱

    // 대화창 스페이스 딜레이
    private float inputDelay = 0.4f;
    private float inputTimer = 0f;


    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("MonologuePopup - Failed to Load PlayerController");
    }

    private void Start()
    {
        monoBox.alpha = 0f;
        monoBox.blocksRaycasts = false;
        monoBox.interactable = false;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.ItemMonologue>(OnItemMonoBox);
        EventBus.Instance.Subscribe<UIEvents.OpenMonologue>(OnOpenMonoBox);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.ItemMonologue>(OnItemMonoBox);
        EventBus.Instance.Unsubscribe<UIEvents.OpenMonologue>(OnOpenMonoBox);
    }

    private void OnItemMonoBox(UIEvents.ItemMonologue evt) {
        preMode = pc.CurMode; // 캐싱

        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그 창 표시
        DialogFade(monoBox, true);
        isOpen = true;

        // 타이핑
        StartCoroutine(TypeItemMonologue(evt.text));
    }

    private void OnOpenMonoBox(UIEvents.OpenMonologue evt) {
        preMode = pc.CurMode; // 캐싱

        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그 창 표시
        DialogFade(monoBox, true);
        isOpen = true;

        // 이벤트 번호 캐싱
        curEventID = evt.eventID;

        // 타이핑
        StartCoroutine(TypeMonologue(evt.texts));
    }

    private void Update()
    {
        if (inputTimer > 0f) // 0.4f 간격으로 입력가능
            inputTimer -= Time.deltaTime;


        if (inputHandler.DoSelect() && inputTimer <= 0f)
        {
            if (isTyping && !isSkip) // 타이핑 중 스킵 한번만 허용 (꼬임방지)
            {
                SkipDialog();
                isSkip = true;
                inputTimer = inputDelay;
            }
            else if (standbyInput && !isTyping) // 입력 대기 상태
            {
                inputTimer = inputDelay;
            }
        }
    }

    // (아이템)독백형 모놀로그 박스
    private IEnumerator TypeItemMonologue(string text)
    {
        seq = DOTween.Sequence();

        sentence = text;
        isTyping = true;
        standbyInput = false;
        isSkip = false;

        // Typing
        float duration = text.Length / typingSpeed;
        typing = monoText.DOText(text, duration).SetEase(Ease.Linear);

        yield return typing.WaitForCompletion(); // 타이핑 완료까지 대기

        isTyping = false;
        standbyInput = true;

        if (isSkip) // 스킵이 눌렸을 때
        {
            yield return null;
            isSkip = false;
        }

        // 입력 대기
        yield return new WaitUntil(() => inputHandler.DoSelect());

        yield return StartCoroutine(ClosePanel());  // 모든 대화가 끝나면 패널 닫음

    }


    // 대화형 모놀로그박스
    private IEnumerator TypeMonologue(Dictionary<int, DialogData> monoDict)
    {
        seq = DOTween.Sequence();

        // 시작 logID의 최소값
        int curlogIdx = monoDict.Keys.Min();

        while (curlogIdx != -1)
        {
            if (!monoDict.TryGetValue(curlogIdx, out var curDialog))
            {
                Debug.Log($"{curlogIdx} : 존재하지 않는 대화 데이터");
                yield break;
            }
            isTyping = true;
            standbyInput = false;
            isSkip = false;
            monoText.text = "";
            sentence = curDialog.dialog; // 캐싱

            // Typing
            float duration = curDialog.dialog.Length / typingSpeed;
            typing = monoText.DOText(curDialog.dialog, duration).SetEase(Ease.Linear);

            yield return typing.WaitForCompletion(); // 타이핑 완료까지 대기

            isTyping = false;
            standbyInput = true;

            if (isSkip) // 스킵이 눌렸을 때
            {
                yield return null;
                isSkip = false;
            }

            // 입력 대기
            yield return new WaitUntil(() => inputHandler.DoSelect());

            // Index++;
            curlogIdx = curDialog.nextID;

        }
        yield return StartCoroutine(ClosePanel());  // 모든 대화가 끝나면 패널 닫음
    }

    // 스킵 시 바로 출력
    private void SkipDialog()
    {
        if (typing != null && typing.IsActive())
        {
            typing.Kill(true);  // 현재 타이핑 애니메이션 즉시 종료, 마지막 상태로 텍스트 완성
        }

        monoText.text = sentence;
        isTyping = false;
        standbyInput = true;
    }

    // 패널 닫기
    public IEnumerator ClosePanel()
    {
        yield return null;
        DialogFade(monoBox, false);

        yield return new WaitForSeconds(0.3f);  //Fade Wait

        // 모드 변경
        pc.CurMode = preMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));

        // 값 초기화
        if (typing != null && typing.IsActive()) typing.Kill();
        if (seq != null && seq.IsActive()) seq.Kill();
        monoText.text = "";

        standbyInput = false;
        isOpen = false;

        yield return null;

        // 대화 끝 이벤트
        EventBus.Instance.Publish<UIEvents.EndDialog>(new UIEvents.EndDialog(curEventID));
    }

    public void DialogFade(CanvasGroup target, bool isDisplay)
    {
        if (target == null) return;

        float val = isDisplay ? 1f : 0f;
        target.interactable = isDisplay;
        target.blocksRaycasts = isDisplay;
        target.DOFade(val, 0.3f).SetEase(Ease.Linear);
    }

}
