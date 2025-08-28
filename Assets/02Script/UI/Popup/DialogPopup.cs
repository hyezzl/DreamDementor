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
    [SerializeField] private float typingSpeed = 20f;

    [Header("UI Refs")]
    [SerializeField] private CanvasGroup playerTextBox;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;

    [SerializeField] private CanvasGroup flipTextBox;
    [SerializeField] private TextMeshProUGUI flipText;
    [SerializeField] private TextMeshProUGUI flipSpeaker;

    [SerializeField] private CanvasGroup enemyTextBox;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI enemySpeaker;

    private PlayerController pc;
    Tweener typing;
    Sequence seq;
    private bool isOpen = false;
    private bool isTyping = false;
    private bool standbyInput = false; // 사용자 입력 기다리기
    private bool isSkip = false;

    private string sentence;  // 캐싱
    private GameMode preMode;  // 캐싱
    private IInputHandler inputHandler;
    private TextMeshProUGUI textarea;  // 사용할 텍스트박스
    private CanvasGroup curTextbox;

    // 대화창 스페이스 연타 시 오류
    private float inputDelay = 0.4f;
    private float inputTimer = 0f;

    [Header("ill")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mom;
    [SerializeField] private Sprite playerill;
    [SerializeField] private Sprite momill;


    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("DialogPopup - Failed to Load PlayerController");
    }

    private void Start()
    {
        // 대화창 모두 숨기기
        SetCanvasGroup(playerTextBox, false);
        SetCanvasGroup(flipTextBox, false);
        SetCanvasGroup(enemyTextBox, false);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OpenDialog>(OnOpenDialog);
        EventBus.Instance.Subscribe<UIEvents.InteractDialog>(OnInteract);
        EventBus.Instance.Subscribe<GameEvents.MakeChoice>(EndChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenDialog>(OnOpenDialog);
        EventBus.Instance.Unsubscribe<UIEvents.InteractDialog>(OnInteract);
        EventBus.Instance.Unsubscribe<GameEvents.MakeChoice>(EndChoice);
    }

    private void Update()
    {
        if (inputTimer > 0f) // 0.4f 간격으로 입력가능
            inputTimer -= Time.deltaTime;
        

        if (inputHandler.DoSelect() && inputTimer <= 0f) {
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

    /// <summary>
    // 외부에서 대화창 열어주는 함수
    /// </summary>
    private void OnOpenDialog(UIEvents.OpenDialog evt) {
        preMode = pc.CurMode; // 캐싱
        
        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그 창 표시
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
            isSkip = false;
            

            

            // Speaker에 따른 분기 (대화창 / 이름 / 폰트)
            if (dialogDict[curlogIdx].speaker == Speaker.Player)  // 화자가 플레이어일 때
            {
                curTextbox = playerTextBox;
                DialogFade(curTextbox, true);
                SetDialog(0, curDialog.speakerName);
            }
            else if (dialogDict[curlogIdx].speaker == Speaker.Enemy) // 화자가 괴물일 때
            {
                curTextbox = enemyTextBox;
                DialogFade(curTextbox, true);
                SetDialog(2, curDialog.speakerName);
            }
            else  // 화자가 그외 (오른쪽에 위치할 인물)
            {
                curTextbox = flipTextBox;
                DialogFade(curTextbox, true);
                SetDialog(1, curDialog.speakerName);
            }

            sentence = curDialog.dialog; // 캐싱


            // Typing
            float duration = curDialog.dialog.Length / typingSpeed;
            typing = textarea.DOText(curDialog.dialog, duration).SetEase(Ease.Linear);
            Debug.Log($"타이핑중 {curDialog.dialog}");

            yield return typing.WaitForCompletion(); // 타이핑 완료까지 대기
            
            isTyping = false;
            standbyInput = true;

            if (isSkip) // 스킵이 눌렸을 때
            {
                yield return null;
                isSkip = false;
            }

            // 선택지가 있으면 이벤트가 발생!
            if (!string.IsNullOrEmpty(curDialog.choiceID)) 
            {
                EventBus.Instance.Publish<UIEvents.OccurSelection>
                    (new UIEvents.OccurSelection(dialogDict[curlogIdx].choices.texts.Count, dialogDict[curlogIdx].choices));
                yield break;  // 선택지 발생 시 대화 멈춤
            }

            // 입력 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            
            // Index++;
            curlogIdx = curDialog.nextID;
        }
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
        DialogFade(curTextbox, false);

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

    public void SetDialog(int popupIdx, string speakerName) {
        // 모든 대화창 비활성화
        SetCanvasGroup(playerTextBox, false);
        SetCanvasGroup(flipTextBox, false);
        SetCanvasGroup(enemyTextBox, false);
        CanvasGroup target = null;

        // 특정 팝업만 활성화
        switch (popupIdx) {
            case 0:
                playerSpeaker.text = speakerName;
                target = playerTextBox;
                textarea = playerText;
                break;
            case 1:
                flipSpeaker.text = speakerName;
                target = flipTextBox;
                textarea = flipText;
                break;
            case 2:
                enemySpeaker.text = speakerName;
                target = enemyTextBox;
                textarea = enemyText;
                break;
        }
        if (target != null) {
            DialogFade(target, true);
        }
        // 텍스트 초기화
        textarea.text = "";
    }

    // CanvasGroup 상태 제어 함수
    private void SetCanvasGroup(CanvasGroup group, bool isActive) {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }

    // group + animation
    public void DialogFade(CanvasGroup target, bool isDisplay)
    {
        float val = isDisplay ? 1f : 0f;
        target.interactable = isDisplay;
        target.blocksRaycasts = isDisplay;
        target.DOFade(val, 0.3f).SetEase(Ease.Linear);
    }

    // 선택지 선택 후
    public void EndChoice(GameEvents.MakeChoice evt) {
        DialogFade(playerTextBox, false);
        DialogFade(flipTextBox, false);
        DialogFade(enemyTextBox, false);
    }

}
