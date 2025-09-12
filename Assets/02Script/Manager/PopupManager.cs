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
    [SerializeField] private GameObject optionContent;
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private GameObject memoContent;
    [SerializeField] private GameObject infoContent;

    [Header("UI Group")]
    [SerializeField] private CanvasGroup popupGroup;
    [SerializeField] private CanvasGroup vertical;
    [SerializeField] private CanvasGroup horizon;

    [Header("Animation")]
    [SerializeField] private AnimationClip openAnim;
    [SerializeField] private AnimationClip closeAnim;


    private Animator anim;
    private IInputHandler inputHandler;
    private PlayerController pc;
    private List<GameObject> contents = new();

    private bool isOpen = false;
    private bool isAnimating = false;       // 애니메이션 중
    private GameMode preMode;       // 이전 모드 캐싱


    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Awake()
    {
        // 초기설정
        background.gameObject.SetActive(false);

        if (!TryGetComponent<Animator>(out anim)) Debug.Log("PopupManager - Failed to Load Animator");
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
        {
            // mode
            Debug.Log("여는 애니메이션 실행!");

            if (!isOpen) // Open
            {
                preMode = pc.CurMode;   // 캐싱
                isOpen = true;

                // 애니메이션 실행
                StartCoroutine(OpenPopupUI());
                background.gameObject.SetActive(true);
                vertical.alpha = 1f;

                // 게임모드 변경
                pc.CurMode = GameMode.PauseMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
            }
            else {
                isOpen = false;
                background.gameObject.SetActive(false);

                // 애니메이션 실행
                StartCoroutine(ClosePopupUI());
                vertical.alpha = 0f;

                // 게임모드 변경
                pc.CurMode = preMode;
                EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
            }
        }
    }


    // 팝업 열리는 애니메이션
    private IEnumerator OpenPopupUI() {
        isAnimating = true;
        popup.gameObject.SetActive(true);

        anim.SetTrigger("OpenPopup");
        yield return new WaitForSeconds(openAnim.length);

        isAnimating = false;
    }

    private IEnumerator ClosePopupUI() {
        isAnimating = true;

        anim.SetTrigger("ClosePopup");
        yield return new WaitForSeconds(closeAnim.length);

        isAnimating = false;
    }
}
