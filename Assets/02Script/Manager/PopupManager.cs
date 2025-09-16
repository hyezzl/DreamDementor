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


public class PopupManager : MonoBehaviour
{
    [Header("UI Contents Refs")]
    [SerializeField] private RectTransform popup;
    [SerializeField] private Image background;
    [SerializeField] private RectTransform vertical;
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
    [SerializeField] private AnimationClip closeAnim;


    private IInputHandler inputHandler;
    private PlayerController pc;
    private List<GameObject> contents = new();

    private bool isOpen = false;
    private bool isAnimating = false;       // 애니메이션 중
    private GameMode preMode;       // 이전 모드 캐싱

    private Vector2 hidePos = new Vector2(0, -830f);
    private Vector2 basisPos = new Vector2(0, -75f);

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        // 초기설정
        background.gameObject.SetActive(false);

        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("PopupManager - Failed to Load PlayerController");
    }

    private void Update()
    {
        if (isAnimating) return;
        TogglePopup();
    }

    private void TogglePopup() {
        if (inputHandler.TogglePopup() || inputHandler.Escape()) // Tab OR ESC
        //if (inputHandler.Escape()) // Tab OR ESC
            {
            if (!isOpen) // Open
            {
                preMode = pc.CurMode;   // 캐싱
                isOpen = true;

                // 애니메이션 실행
                StartCoroutine(OpenPopupUI());

                // 게임모드 변경
                pc.CurMode = GameMode.PauseMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
            }
            else {
                isOpen = false;

                // 애니메이션 실행
                StartCoroutine(ClosePopupUI());

                // 게임모드 변경
                pc.CurMode = preMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
            }
        }
    }


    // 팝업 열리는 애니메이션
    private IEnumerator OpenPopupUI() {
        isAnimating = true;
        isOpen = true;
        popup.gameObject.SetActive(true);
        OnOffgroup(animGroup, true);
        background.gameObject.SetActive(true);

        anim.SetTrigger("OpenPopup");
        yield return new WaitForSeconds(openAnim.length);

        OnOffgroup(verticalGroup, true);
        OnOffgroup(animGroup, false);
        isAnimating = false;
        anim.SetBool("isOpen", isOpen);
    }

    private IEnumerator ClosePopupUI() {
        isAnimating = true;

        yield return vertical.DOAnchorPos(hidePos, 0.8f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        OnOffgroup(verticalGroup, false);
        isAnimating = false;
        isOpen = false;
        background.gameObject.SetActive(false);
        anim.SetBool("isOpen", isOpen);
        vertical.anchoredPosition = basisPos;
    }


    private void OnOffgroup(CanvasGroup group, bool isOn)
    {
        group.alpha = isOn ? 1 : 0;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }
}
