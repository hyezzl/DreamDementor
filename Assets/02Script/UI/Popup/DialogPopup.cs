using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Sequence = DG.Tweening.Sequence;

public class DialogPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup popup;
    [SerializeField] private TextMeshProUGUI textarea;
    [SerializeField] private TextMeshProUGUI speaker;
    [SerializeField] private float typingSpeed = 20f;

    private PlayerController pc;
    Tweener typing;
    Sequence seq;
    private bool isOpen = false;
    private bool isTyping = false;
    private bool standbyInput = false; // 사용자 입력 기다리기
    private bool isSkip = false;

    private string sentence;  // 캐싱
    private GameMode preMode;  // 캐싱

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("DialogPopup - Failed to Load PlayerController");
    }

    private void Start()
    {
        popup.interactable = false;
        popup.blocksRaycasts = false;
        popup.alpha = 0f;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OpenDialog>(OnOpenDialog);
        EventBus.Instance.Subscribe<UIEvents.InteractDialog>(OnInteract);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenDialog>(OnOpenDialog);
        EventBus.Instance.Unsubscribe<UIEvents.InteractDialog>(OnInteract);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isTyping)
            {
                SkipDialog();
                isSkip = true;
            } 
            //else if (standbyInput) // 입력 대기 상태
            //{
            //    // delete blink

            //    StartCoroutine(ClosePanel());
            //}
        }
    }


    private void LogInit() {
        speaker.text = "";
        textarea.text = "";
    }

    /// <summary>
    // 외부에서 대화창 열어주는 함수
    /// </summary>
    private void OnOpenDialog(UIEvents.OpenDialog evt) {
        preMode = pc.CurMode; // 캐싱
        
        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그 창 표시
        DialogFade(true);
        isOpen = true;

        // 타이핑
        StartCoroutine(TypeDialog(evt.texts));
    }


    /// <summary>
    /// 아이템 상호작용 시 대화창
    /// </summary>
    private void OnInteract(UIEvents.InteractDialog evt) {
        preMode = pc.CurMode; // 캐싱

        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

    }


    public void DialogFade(bool isDisplay) {
        float val = isDisplay ? 1f : 0f;
        popup.interactable = isDisplay;
        popup.blocksRaycasts = isDisplay;
        popup.DOFade(val, 0.8f).SetEase(Ease.Linear);
    }

    private IEnumerator TypeDialog(Dictionary<int, DialogData> dialogDict) {
        seq = DOTween.Sequence();

        // 시작 logID의 최소값
        int curlogIdx = dialogDict.Keys.Min();

        while (curlogIdx != -1) {
            if (!dialogDict.TryGetValue(curlogIdx, out var curDialog)) {
                Debug.Log($"{curlogIdx} : 존재하지 않는 대화 데이터");
                yield break;
            }

            isTyping = true;
            standbyInput = false;
            LogInit();
            sentence = curDialog.dialog; // 캐싱

            // Speaker
            speaker.text = curDialog.speakerName;

            // Typing
            float duration = curDialog.dialog.Length / typingSpeed;
            typing = textarea.DOText(curDialog.dialog, duration).SetEase(Ease.Linear);

            yield return typing.WaitForCompletion(); // 타이핑 완료까지 대기
            
            isTyping = false;
            standbyInput = true;

            if (isSkip) // 스킵이 눌렸을 때
            {
                yield return null;
                isSkip = false;
            }

            // 선택지가 있으면 이벤트가 발생!
            if (!string.IsNullOrEmpty(curDialog.choiceID)) {
                EventBus.Instance.Publish<UIEvents.OccurSelection>(new UIEvents.OccurSelection(dialogDict[curlogIdx].choices.texts.Count));
                yield break;  // 선택지 발생 시 대화 멈춤
            }

            // 입력 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        
        // 선택지 발생
        //EventBus.Instance.Publish<UIEvents.OpenSelectBox>(new UIEvents.OpenSelectBox(0));
    }

    // 스킵 시 바로 출력
    private void SkipDialog()
    {
        if (typing != null && typing.IsActive())
        {
            typing.Kill(true);  // 현재 타이핑 애니메이션 즉시 종료, 마지막 상태로 텍스트 완성
        }

        textarea.text = sentence;
        isTyping = false;
        standbyInput = true;
        //if (blinkCor != null) StopCoroutine(blinkCor);
        //blinkCor = StartCoroutine(blink.BlinkAnnounceMSG(group));
    }

    // 패널 닫기
    public IEnumerator ClosePanel() {
        yield return null;
        DialogFade(false);

        // 모드 변경
        pc.CurMode = preMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));

        // 값 초기화
        if (typing != null && typing.IsActive()) typing.Kill();
        if (seq != null && seq.IsActive()) seq.Kill();
        textarea.text = "";

        standbyInput = false;
        isOpen = false;
    }

}
