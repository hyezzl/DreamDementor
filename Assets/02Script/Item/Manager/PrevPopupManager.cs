using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class PrevPopupManager : MonoBehaviour
{
    [Header("UI BTN Refs")]
    [SerializeField] private List<Button> btns;

    [Header("UI Content Refs")]
    [SerializeField] private RectTransform popup;
    [SerializeField] private List<GameObject> contents;
    [SerializeField] private Image background;

    [Header("Popup Location")]
    [SerializeField] private Vector2 displayPos;
    [SerializeField] private Vector2 hidePos;

    private PlayerController pc;
    private IInputHandler inputHandler;
    private GameMode preMode; // 이전모드 캐싱
    private bool isOpen = false;

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PopupManager - Failed to Load PlayerController");
    }

    private void Start()
    {
        popup.anchoredPosition = hidePos;
    }

    private void Update()
    {
        TogglePopup();
    }

    // 각 버튼에 이벤트 연결
    private void OnEnable()
    {
        //Tab BTNList
        for (int i = 0; i < 4; i++) {
            int idx = i;
            btns[idx].onClick.AddListener(() => ChangeContent((PopupContent)idx));
        }
    }
    private void OnDisable()
    {
        foreach (var btn in btns) {
            btn.onClick.RemoveAllListeners();
        }
    }


    // 팝업 토글
    private void TogglePopup() {
        if (inputHandler.TogglePopup()) // Tab
        {
            // todo : mode 조건 분기
            if (!isOpen) // Open
            {
                preMode = pc.CurMode; // 캐싱
                isOpen = true;
                PopupAnimation();
                background.gameObject.SetActive(true);

                // 게임모드 변경
                pc.CurMode = GameMode.PauseMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
            }
            else { 
                isOpen = false;
                PopupAnimation();
                background.gameObject.SetActive(false);
                

                // 게임모드 변경
                pc.CurMode = preMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
            }
        }
    }

    // 팝업 애니메이션 (시간이 멈춰도 실행되어야 함)
    private void PopupAnimation() {
        popup.DOKill();  // 기존 tween 즉시중지
        if (isOpen)
        {
            popup.DOAnchorPos(displayPos, 0.5f).SetUpdate(true)
                .SetEase(Ease.OutBack);
        }
        else {
            popup.DOAnchorPos(hidePos, 0.5f).SetUpdate(true)
                .SetEase(Ease.OutBack);
        }
    }

    private void ChangeContent(PopupContent type) {
        foreach (var content in contents) { 
            content.SetActive(false);
        }
        contents[(int)type].SetActive(true);
    }
}
