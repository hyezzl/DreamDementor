using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class NpcDialogPopup : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 20f;

    [Header("UI Refs")]
    [SerializeField] private CanvasGroup basicTextBox;
    [SerializeField] private TextMeshProUGUI basicText;
    [SerializeField] private TextMeshProUGUI monologueText;
    [SerializeField] private TextMeshProUGUI basicSpeaker;

    [SerializeField] private CanvasGroup enemyTextBox;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI enemySpeaker;

    [SerializeField] private CanvasGroup LeftIll; // alpha값 조절.
    [SerializeField] private SkeletonGraphic playerSpine;
    [SerializeField] private CanvasGroup RightIll;
    [SerializeField] private Image otherIll;

    private PlayerController pc;
    Tweener typing;
    Sequence seq;
    private bool isOpen = false;
    private bool isTyping = false;
    private bool standbyInput = false; // 사용자 입력 기다리기
    private bool isSkip = false;

    private string curNpcID;    // 캐싱
    private string curNpcEventID;   // 캐싱
    private string sentence;  // 캐싱
    private GameMode preMode;  // 캐싱
    private GameMode afterMode;     // 설정값
    private IInputHandler inputHandler;
    private TextMeshProUGUI textarea;  // 사용할 텍스트박스
    private CanvasGroup curTextbox;
    private int curPanelIndex = -1;  // 현재 적용되어있는 대화창 UI

    // 대화창 스페이스 연타 시 오류
    private float inputDelay = 0.4f;
    private float inputTimer = 0f;

    // 일러스트 변경 시 최적화
    private int curLeftIdx = -1;  // 주인공은 무조건 왼쪽
    private Speaker curRight = Speaker.Enemy;             // 오른쪽은 누구나 가능
    private int curRightIdx = -1;                         // 이모션 인덱스
    private Color deactive = new Color(0.4f, 0.4f, 0.4f, 1f);

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("DialogPopup - Failed to Load PlayerController");
    }
    private void Start()
    {
        // 대화창 모두 숨기기
        SetCanvasGroup(basicTextBox, false);
        SetCanvasGroup(enemyTextBox, false);

        LeftIll.alpha = 0f;
        SetColor(true);

        // 투명처리
        otherIll.color = new Color(1, 1, 1, 0);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OpenNpcDialog>(OnOpenNpcDialog);
        EventBus.Instance.Subscribe<UIEvents.OpenNpcReDialog>(OnOpenNpcReDialog);
        EventBus.Instance.Subscribe<UIEvents.CloseDialog>(CloseDialogPanel);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OpenNpcDialog>(OnOpenNpcDialog);
        EventBus.Instance.Unsubscribe<UIEvents.OpenNpcReDialog>(OnOpenNpcReDialog);
        EventBus.Instance.Unsubscribe<UIEvents.CloseDialog>(CloseDialogPanel);
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

    /// <summary>
    /// NPC 대화
    /// </summary>
    private void OnOpenNpcDialog(UIEvents.OpenNpcDialog evt)
    {
        afterMode = evt.afterMode;      // 설정 모드 캐싱

        // 혹시모를 초기화
        if (playerSpine != null)
        {
            SetColor(true);
        }

        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그창 표시
        isOpen = true;

        // NPC 
        curNpcID = evt.npcID;
        curNpcEventID = evt.npcEventID;

        // 타이핑
        StartCoroutine(TypeDialog(evt.texts));
    }

    private void OnOpenNpcReDialog(UIEvents.OpenNpcReDialog evt) {
        preMode = pc.CurMode;

        // 모드 변경
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        // 로그창 표시
        isOpen = true;

        // NPC 
        curNpcID = evt.npcID;
        curNpcEventID = null;

        // 타이핑
        StartCoroutine(TypeReDialog(evt.data));
    }


    private IEnumerator TypeDialog(Dictionary<int, NPCDialogData> npcDialogDict)
    {
        seq = DOTween.Sequence();

        // 시작 logID의 최소값
        int curlogIdx = npcDialogDict.Keys.Min();

        while (curlogIdx != -1)
        {
            if (!npcDialogDict.TryGetValue(curlogIdx, out var curDialog))
            {
                Debug.Log($"{curlogIdx} : 존재하지 않는 대화 데이터");
                yield break;
            }

            isTyping = true;
            standbyInput = false;
            isSkip = false;
            //ShowIllust(npcDialogDict, curlogIdx);

            // Textbox에 따른 분기 (대화창 / 폰트)
            if (npcDialogDict[curlogIdx].textbox == Textbox.Basic || npcDialogDict[curlogIdx].textbox == Textbox.Monologue)
            {
                curTextbox = basicTextBox;
                DialogFade(curTextbox, true);
                SetDialog(0, curDialog.speakerName, curDialog.textbox);

                ShowIllust(npcDialogDict, curlogIdx);
            }
            else if (npcDialogDict[curlogIdx].textbox == Textbox.Monster)  // 몬스터 텍스트 박스
            {
                curTextbox = enemyTextBox;
                DialogFade(curTextbox, true);
                SetDialog(1, curDialog.speakerName, curDialog.textbox);

                ShowIllust(npcDialogDict, curlogIdx);
            }

            sentence = curDialog.dialog; // 캐싱

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
            if (!string.IsNullOrEmpty(curDialog.choiceID) && curDialog.choices != null)
            {
                EventBus.Instance.Publish<UIEvents.OccurSelection>
                    (new UIEvents.OccurSelection(npcDialogDict[curlogIdx].choices.texts.Count, npcDialogDict[curlogIdx].choices, true));
                yield break;  // 선택지 발생 시 대화 멈춤
            }

            // 입력 대기
            yield return new WaitUntil(() => inputHandler.DoSelect());

            // Index++;
            curlogIdx = curDialog.nextID;
        }

        yield return StartCoroutine(ClosePanel());  // 모든 대화가 끝나면 패널 닫음

        // 모드 변경
        if (afterMode != GameMode.None)
        {
            // 대화 진입 시 게임모드 지정
            pc.CurMode = afterMode;
            Debug.Log($"ClosePanel에서 상태변경 : {afterMode}로!");
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(afterMode));
        }
        else
        {
            // 대화 진입 시 따로 게임모드를 정해주지 않은 경우 (Inspector고정)
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }
    }


    private IEnumerator TypeReDialog(NPCReDialogData data)
    {
        seq = DOTween.Sequence();

        sentence = data.dialog;
        isTyping = true;
        standbyInput = false;
        isSkip = false;

        // Textbox에 따른 분기 (대화창 / 폰트)
        if (data.textbox == Textbox.Basic || data.textbox == Textbox.Monologue)
        {
            curTextbox = basicTextBox;
            DialogFade(curTextbox, true);
            SetDialog(0, data.speakerName, data.textbox);

        }
        else if (data.textbox == Textbox.Monster)  // 몬스터 텍스트 박스
        {
            curTextbox = enemyTextBox;
            DialogFade(curTextbox, true);
            SetDialog(1, data.speakerName, data.textbox);

        }

        // Typing
        float duration = sentence.Length / typingSpeed;
        typing = textarea.DOText(sentence, duration).SetEase(Ease.Linear);

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


        // NPCReDialog 이후엔 반드시 inspect모드임을 전제!!!!!!!!!!!!!!
        pc.CurMode = GameMode.InspectMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
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
    }

    // 패널 닫기
    public IEnumerator ClosePanel()
    {
        yield return null;
        DialogFade(curTextbox, false);

        yield return new WaitForSeconds(0.3f);  //Fade Wait

        // 값 초기화
        if (typing != null && typing.IsActive()) typing.Kill();
        if (seq != null && seq.IsActive()) seq.Kill();
        if(textarea != null)
            textarea.text = "";

        // Spine 리셋
        if (playerSpine != null)
        {
            SetColor(true);     // 스파인 색상초기화        
        }

        // 일러스트 닫음 +  초기화
        LeftIll.alpha = 0f;
        RightIll.alpha = 0f;
        otherIll.sprite = null;
        otherIll.color = new Color(1, 1, 1, 0);

        standbyInput = false;
        isOpen = false;

        yield return null;

        // 대화끝 이벤트 (대화 NPC ID 전달)
        EventBus.Instance.Publish<UIEvents.EndNpcDialog>(new UIEvents.EndNpcDialog(curNpcID, curNpcEventID));
    }

    // 외부에서 강제 대화창 닫음
    private void CloseDialogPanel(UIEvents.CloseDialog evt) {
        if (evt.isNpc) {
            Debug.Log("npc창 닫습니다");
            StartCoroutine(ClosePanel());
        }
    }



    public void SetDialog(int popupIdx, string speakerName, Textbox boxType)
    {
        // 이전에 띄웠던 타입과 같으면 패널 그대로 둠
        // case 0 (basic)  / case 1 (monster)
        if (curPanelIndex != -1 && curPanelIndex == popupIdx)
        {
            switch (popupIdx)
            {
                case 0:  // basic
                    basicSpeaker.text = speakerName;
                    if (boxType == Textbox.Basic)
                    {
                        textarea = basicText;
                        basicText.enabled = true;
                        monologueText.enabled = false;
                    }
                    else
                    {
                        textarea = monologueText;
                        basicText.enabled = false;
                        monologueText.enabled = true;
                    }
                    break;

                case 1:  // enemy
                    enemySpeaker.text = speakerName;
                    textarea = enemyText;
                    break;
            }
            textarea.text = ""; // 텍스트만 초기화
            return;
        }


        // 패널 바뀌는 경우
        SetCanvasGroup(basicTextBox, false);
        SetCanvasGroup(enemyTextBox, false);
        CanvasGroup target = null;

        // 특정 팝업만 활성화
        switch (popupIdx)
        {
            case 0:
                basicSpeaker.text = speakerName;
                if (boxType == Textbox.Basic)
                {
                    textarea = basicText;
                    basicText.enabled = true;
                    monologueText.enabled = false;
                }
                else
                {
                    textarea = monologueText;
                    basicText.enabled = false;
                    monologueText.enabled = true;
                }
                break;

            case 1:
                enemySpeaker.text = speakerName;
                target = enemyTextBox;
                textarea = enemyText;
                break;
        }
        if (target != null)
        {
            DialogFade(target, true);
        }
        // 텍스트 초기화
        textarea.text = "";
        curPanelIndex = popupIdx; // 캐싱
    }

    // CanvasGroup 상태 제어 함수 (즉발)
    private void SetCanvasGroup(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }

    // group + animation
    public void DialogFade(CanvasGroup target, bool isDisplay)
    {
        if (target == null || target.gameObject == null) return;

        float val = isDisplay ? 1f : 0f;
        target.interactable = isDisplay;
        target.blocksRaycasts = isDisplay;
        target.DOFade(val, 0.3f).SetEase(Ease.Linear);
    }

    // 스프라이트 Addressable로 비동기 로드
    public void LoadSprite(string address, Image targetImg)
    {
        // 현재 스프라이트와 같으면 교체하지 않음
        if (targetImg.sprite != null && targetImg.sprite.name == address)
        {
            return;
        }

        targetImg.color = new Color(1, 1, 1, 0); // 투명

        /// 첫 일러스트 나올 때 버벅임 삭제
        targetImg.DOKill();  // 현재 진행 중인 페이드 애니메이션 중지
        targetImg.DOFade(0f, 0f);  // 즉시 투명 처리

        Addressables.LoadAssetAsync<Sprite>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                targetImg.sprite = handle.Result;
                targetImg.color = Color.white;
                targetImg.DOFade(1f, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                Debug.Log("대화창 스프라이트 로드 실패");
                targetImg.color = new Color(1, 1, 1, 0);
            }
        };
    }


    private void ShowIllust(Dictionary<int, NPCDialogData> npcDialogDict, int curlogIdx) {
        // 일러스트
        if (npcDialogDict[curlogIdx].emotion == -1)    // 일러스트 없을 경우
        {
            if (npcDialogDict[curlogIdx].speaker == Speaker.Player)
            {
                LeftIll.alpha = 0f;
                otherIll.color = deactive;
            }
            else
            {
                RightIll.alpha = 0f;
                SetColor(false);
            }
            return;   // 일러스트 없을 경우
        }

        switch (npcDialogDict[curlogIdx].speaker)
        {
            case Speaker.Player:
                if (curLeftIdx != npcDialogDict[curlogIdx].emotion)
                {
                    curLeftIdx = npcDialogDict[curlogIdx].emotion; // cache
                    string animName = ((PlayerEmotion)npcDialogDict[curlogIdx].emotion).ToString();

                    if (playerSpine != null)
                    {
                        SetColor(true);
                        PlayAnim(animName);
                    }
                }
                LeftIll.alpha = 1f;
                SetColor(true);
                otherIll.color = deactive;
                break;

            case Speaker.Mom:
                //if (curRight != npcDialogDict[curlogIdx].speaker && curRightIdx != npcDialogDict[curlogIdx].emotion)
                if (curRight != npcDialogDict[curlogIdx].speaker || curRightIdx != npcDialogDict[curlogIdx].emotion)
                {
                    curRight = Speaker.Mom; // cache
                    var curAddress = (MomEmotion)npcDialogDict[curlogIdx].emotion;
                    curRightIdx = npcDialogDict[curlogIdx].emotion; // cache

                    LoadSprite(npcDialogDict[curlogIdx].speaker.ToString() + "/" + curAddress.ToString()
                                , otherIll);
                }
                RightIll.alpha = 1f;
                otherIll.color = Color.white;
                SetColor(false);
                break;

            case Speaker.Enemy:
                //if (curRight != npcDialogDict[curlogIdx].speaker && curRightIdx != npcDialogDict[curlogIdx].emotion)
                if (curRight != npcDialogDict[curlogIdx].speaker || curRightIdx != npcDialogDict[curlogIdx].emotion)
                {
                    curRight = Speaker.Enemy;
                    var curAddress = (EnemyEmotion)npcDialogDict[curlogIdx].emotion;
                    curRightIdx = npcDialogDict[curlogIdx].emotion;

                    LoadSprite(npcDialogDict[curlogIdx].speaker.ToString() + "/" + curAddress.ToString()
                                , otherIll);
                }
                RightIll.alpha = 1f;
                otherIll.color = Color.white;
                SetColor(false);
                break;

            case Speaker.Extra:
                //if (curRight != npcDialogDict[curlogIdx].speaker && curRightIdx != npcDialogDict[curlogIdx].emotion)
                if (curRight != npcDialogDict[curlogIdx].speaker || curRightIdx != npcDialogDict[curlogIdx].emotion)
                {
                    curRight = Speaker.Extra;
                    var curAddress = (ExtraEmotion)npcDialogDict[curlogIdx].emotion;
                    curRightIdx = npcDialogDict[curlogIdx].emotion;

                    LoadSprite(npcDialogDict[curlogIdx].speaker.ToString() + "/" + curAddress.ToString()
                                , otherIll);
                }
                RightIll.alpha = 1f;
                otherIll.color = Color.white;
                SetColor(false);
                break;
        }
    }

    // 스파인 애니메이션 재생 함수
    private void PlayAnim(string name, bool loop = true)
    {
        if (playerSpine != null)
        {
            playerSpine.AnimationState.SetAnimation(0, name, loop);
        }
    }

    // 스파인 활성/비활성 색 함수
    private void SetColor(bool isActive)
    {
        if (playerSpine != null)
        {
            playerSpine.color = isActive ? Color.white : deactive;
        }
    }
}
