using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PopupContent
{
    inventory,
    quest,
    option,
    save,
}

public enum PopupLayer
{ 
    None,
    Vertical,
    Horizontal,
    InvenDesc,
}


public class PopupManager : MonoBehaviour
{
    [Header("UI Contents Refs")]
    [SerializeField] private RectTransform popup;
    [SerializeField] private Image background;
    [SerializeField] private RectTransform vertical;
    [SerializeField] private RectTransform horizontal;
    [SerializeField] private GameObject optionContent;
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private GameObject memoContent;
    [SerializeField] private GameObject infoContent;

    [Header("UI Group")]
    [SerializeField] private CanvasGroup popupGroup;
    [SerializeField] private CanvasGroup animGroup;
    [SerializeField] private CanvasGroup verticalGroup;
    [SerializeField] private CanvasGroup horizonGroup;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip openAnim;


    private IInputHandler inputHandler;
    private PlayerController pc;

    private PopupLayer curLayer = PopupLayer.None;    // 현재 팝업 레이어
    private bool isOpen = false;
    private bool isAnimating = false;       // 애니메이션 중
    private GameMode preMode;       // 이전 모드 캐싱

    private Vector2 hidePos = new Vector2(0, -1000f);
    private Vector2 basisPos = new Vector2(0, -75f);

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PopupManager - Failed to Load PlayerController");
    }

    private void Start()
    {
        // 초기설정
        background.gameObject.SetActive(false);

        optionContent.SetActive(true);
        inventoryContent.SetActive(true);
        memoContent.SetActive(true);
        infoContent.SetActive(true);
    }

    private void Update()
    {
        // Inspector모드에서만 가능
        if (pc.CurMode != GameMode.InspectMode && pc.CurMode != GameMode.PauseMode) return;

        if (isAnimating) return;
        TogglePopup();
    }

    private void TogglePopup() {
        if (inputHandler.TogglePopup() || inputHandler.Escape()) // Tab OR ESC
        {
            if (!isOpen) // Open
            {
                if(pc.CurMode != GameMode.PauseMode)
                    preMode = pc.CurMode;   // 캐싱
                isOpen = true;

                // 애니메이션 실행
                StartCoroutine(OpenVerticalUI());

                // 게임모드 변경
                pc.CurMode = GameMode.PauseMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
            }
            else 
            {
                if (curLayer == PopupLayer.Horizontal)
                {
                    // 가로상태에서 esc누르면 vertical
                    ShowVertical();
                }
                else if (curLayer == PopupLayer.Vertical) { 
                    ClosePopup();
                }

                isOpen = false;
            }
        }
    }


    // 팝업 열리는 애니메이션
    private IEnumerator OpenVerticalUI() {
        isAnimating = true;
        isOpen = true;
        popup.gameObject.SetActive(true);
        popupGroup.alpha = 1f;
        curLayer = PopupLayer.Vertical;

        OnOffgroup(animGroup, true);
        background.gameObject.SetActive(true);

        anim.SetTrigger("OpenPopup");
        yield return new WaitForSeconds(openAnim.length);

        OnOffgroup(verticalGroup, true);
        OnOffgroup(horizonGroup, false);
        OnOffgroup(animGroup, false);
        isAnimating = false;
        anim.SetBool("isOpen", isOpen);
    }

    // 팝업닫는 코루틴
    public IEnumerator ClosePopupUI() {
        isAnimating = true;

        switch (curLayer)
        {
            case PopupLayer.Vertical:
                yield return vertical.DOAnchorPos(hidePos, 0.8f)
                .SetEase(Ease.InOutSine)
                .WaitForCompletion();

                vertical.anchoredPosition = basisPos;
                break;

            case PopupLayer.Horizontal:
                yield return horizontal.DOAnchorPos(hidePos, 0.8f)
                .SetEase(Ease.InOutSine)
                .WaitForCompletion();

                horizontal.anchoredPosition = basisPos;
                break;
        }

        OnOffgroup(verticalGroup, false);
        OnOffgroup(horizonGroup, false);

        isAnimating = false;
        isOpen = false;
        background.gameObject.SetActive(false);
        anim.SetBool("isOpen", isOpen);
        //vertical.anchoredPosition = basisPos;
    }


    private void OnOffgroup(CanvasGroup group, bool isOn)
    {
        group.alpha = isOn ? 1 : 0;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }

    // Verical <-> Horizontal
    public void ShowVertical() {
        curLayer = PopupLayer.Vertical;
        OnOffgroup(verticalGroup, true);
        OnOffgroup(horizonGroup, false);
    }

    public void ShowHorizontal() {
        curLayer = PopupLayer.Horizontal;
        OnOffgroup(verticalGroup, false);
        OnOffgroup(horizonGroup, true);
    }


    // 최종적으로 팝업 닫기
    public void ClosePopup() {
        if (isAnimating) return;
        StartCoroutine(ClosePopupUI());
        curLayer = PopupLayer.None;

        // 게임모드 변경
        pc.CurMode = preMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
    }
}
