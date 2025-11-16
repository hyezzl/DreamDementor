using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PhoneUIManager : MonoBehaviour
{
    [Header("UI Contents Refs")]
    [SerializeField] private RectTransform popup;
    [SerializeField] private Image background;
    [SerializeField] private RectTransform phone;

    [SerializeField] private GameObject home;
    [SerializeField] private GameObject content;

    [SerializeField] private GameObject optionContent;
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private GameObject memoContent;
    [SerializeField] private GameObject infoContent;
    [SerializeField] private InventoryDescription invenDesc;    // 인벤토리 세부창


    private IInputHandler inputHandler;
    private bool isOpen = false;
    public bool isAnimating = false;       // 애니메이션 중
    private GameMode preMode;       // 이전 모드 캐싱

    private Vector2 hidePos = new Vector2(0, -1000f);
    private Vector2 basisPos = new Vector2(0, 0f);

    // 딜레이
    private float inputBlockDuration = 0.3f;
    private bool inputBlocked = false;

    public void SetInputHandler(IInputHandler inputHandler) => this.inputHandler = inputHandler;

    private void Start()
    {
        // 초기설정
        background.gameObject.SetActive(false);
        phone.anchoredPosition = hidePos;

        //optionContent.SetActive(true);
        //inventoryContent.SetActive(true);
        //memoContent.SetActive(true);
        //infoContent.SetActive(true);
    }

    private void Update()
    {
        // Inspector모드에서만 가능
        if (PlayerController.Instance.CurMode != GameMode.InspectMode && PlayerController.Instance.CurMode != GameMode.PauseMode) return;

        if (isAnimating) return;
        if (inputBlocked) return;

        if (inputHandler.TogglePopup() || inputHandler.Escape())
        {
            StartCoroutine(BlockInputAndToggle());
        }
    }

    private void TogglePopup()
    {
        if (!isOpen)
        {
            if (PlayerController.Instance.CurMode != GameMode.PauseMode)
                preMode = PlayerController.Instance.CurMode;   // 캐싱

            // 애니메이션 실행
            StartCoroutine(OpenPopupUI());

            // 게임모드 변경
            PlayerController.Instance.CurMode = GameMode.PauseMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.PauseMode));
        }
        else
        {
            StartCoroutine(ClosePopupUI());

            isOpen = false;
        }
    }



    // 팝업 열리는 애니메이션
    private IEnumerator OpenPopupUI()
    {
        isAnimating = true;
        popup.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        GotoHome();     // 무조건 홈화면으로 시작

        yield return phone.DOAnchorPos(basisPos, 0.8f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();


        isAnimating = false;
        isOpen = true;
    }

    // 팝업닫는 코루틴
    public IEnumerator ClosePopupUI()
    {
        isAnimating = true;
        background.gameObject.SetActive(false);

        // 닫기직전 혹시, Description 켜져있다면 꺼줌
        if (invenDesc != null) {
            invenDesc.CloseDescription();
        }

        yield return phone.DOAnchorPos(hidePos, 0.8f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        //OnOffgroup(popupGroup, false);
        isAnimating = false;
        isOpen = false;

        // 애니메이션이 모두 끝난 후
        GotoHome();     // 홈화면으로 초기화

        // 게임모드 변경
        PlayerController.Instance.CurMode = preMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
    }


    // 홈 > 앱
    public void IntoApp() {
        home.SetActive(false);
        content.SetActive(true);
    }


    // 앱 > 홈
    public void GotoHome() { 
        content.SetActive(false);
        home.SetActive(true);
    }




    private void OnOffgroup(CanvasGroup group, bool isOn)
    {
        group.alpha = isOn ? 1 : 0;
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }

    private IEnumerator BlockInputAndToggle()
    {
        inputBlocked = true;

        // 기존 토글 동작 호출
        TogglePopup();

        // 딜레이 후 입력 차단 해제
        yield return new WaitForSeconds(inputBlockDuration);
        inputBlocked = false;
    }
}
