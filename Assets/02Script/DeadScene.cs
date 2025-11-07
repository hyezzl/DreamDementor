using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeadScene : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    //현재 죽음에 대한 정보 캐싱값
    private SceneType scene;
    private DeathType deathType;

    [SerializeField] private float fadeDuration = 3f;

    [Header("UI Ref")]
    [SerializeField] private CanvasGroup deadScene;
    [SerializeField] private CanvasGroup black;
    [SerializeField] private Image illust;
    [SerializeField] private TextMeshProUGUI overText;
    [SerializeField] private CanvasGroup overGroup;

    // 버튼
    [SerializeField] private Button loadBTN;
    [SerializeField] private Image loadback;
    [SerializeField] private CanvasGroup BTNGroup;

    [SerializeField] private Button exitBTN;
    [SerializeField] private Image exitback;


    private void OnEnable()
    {
        // 초기값
        BTNGroup.gameObject.SetActive(false);
        black.gameObject.SetActive(false);
        illust.gameObject.SetActive(false);
        overGroup.gameObject.SetActive(false);

        EventBus.Instance.Subscribe<GameEvents.GameOver>(OnOver);
        loadBTN.onClick.AddListener(OnLoadGame);
        exitBTN.onClick.AddListener(ExitGame);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameOver>(OnOver);
        loadBTN.onClick.RemoveListener(OnLoadGame);
        exitBTN.onClick.RemoveListener(ExitGame);
    }

    private void OnOver(GameEvents.GameOver evt) {
        // 게임모드
        PlayerController.Instance.CurMode = GameMode.GameOverMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));

        // 게임오버 정보 캐싱
        scene = evt.scene;
        deathType = evt.type;

        // 데드씬 연출
        StartCoroutine(DeadSceneCor());
    }

    private IEnumerator DeadSceneCor() {
        // 효과음
        deadScene.alpha = 1f;

        // 검은배경 페이드인
        //yield return StartCoroutine(Fade(black, 0f, 1f, 1f));

        black.gameObject.SetActive(true);
        black.alpha = 1f;   // 페이드인보다는 바로 까매지는게 나은 듯

        yield return new WaitForSeconds(1f);

        // fade아웃 직전에 일러스트 켜줌
        illust.gameObject.SetActive(true);

        // 검은배경 페이드아웃
        yield return StartCoroutine(Fade(black, 1f, 0f, 3f));

        yield return new WaitForSeconds(2f);        // 2초동안 일러스트 보여줌

        // 검은배경 페이드인
        yield return StartCoroutine(Fade(black, 0f, 0.8f, 2f));

        // 글자 페이드인 // 3
        overGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(overGroup, 0f, 1f, 3f));

        // 마우스 생성
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        // 버튼 활성화
        BTNGroup.gameObject.SetActive(true);
        StartCoroutine(Fade(BTNGroup, 0f, 1f, 1f));
        BTNGroup.interactable = true;
    }


    private IEnumerator AfterClick() { 
        // 버튼 제거
        BTNGroup.gameObject.SetActive(false);

        // 검은배경으로 서서히 채워짐
        StartCoroutine(Fade(overGroup, 1f, 0f, 0.8f));
        yield return StartCoroutine(Fade(black, 0.8f, 1f, 1f));
    }


    public IEnumerator Fade(CanvasGroup group, float startVal, float endVal, float fadeDuration)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startVal, endVal, elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = endVal;
    }


    private void OnLoadGame() {
        StartCoroutine(LoadBTN());
    }

    private IEnumerator LoadBTN() {
        // 로드 버튼 눌렸을때
        Debug.Log("게임이 로드 됩니다!!!!!!!!!!!!!!!!!!!!!!!!!");
        yield return StartCoroutine(AfterClick());

        SaveManager.Instance.ReloadScene(SwitchSceneManager.Instance.CurScene);
    }

    /// 버튼

    // loadBTN 전용
    //public void OnLoadButtonPointerEnter(PointerEventData eventData)
    //{
    //    if (backImg != null)
    //        backImg.enabled = true;
    //}
    //public void OnLoadButtonPointerExit(PointerEventData eventData)
    //{
    //    if (backImg != null)
    //        backImg.enabled = false;
    //}

    //// exitBTN 전용
    //public void OnExitButtonPointerEnter(PointerEventData eventData)
    //{
    //    if (backImg2 != null)
    //        backImg2.enabled = true;
    //}
    //public void OnExitButtonPointerExit(PointerEventData eventData)
    //{
    //    if (backImg2 != null)
    //        backImg2.enabled = false;
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (eventData.pointerEnter == loadBTN.gameObject)
    //    {
    //        if (backImg != null) backImg.enabled = true;
    //    }
    //    else if (eventData.pointerEnter == exitBTN.gameObject)
    //    {
    //        if (backImg2 != null) backImg2.enabled = true;
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (eventData.pointerEnter == loadBTN.gameObject)
    //    {
    //        if (backImg != null) backImg.enabled = false;
    //    }
    //    else if (eventData.pointerEnter == exitBTN.gameObject)
    //    {
    //        if (backImg2 != null) backImg2.enabled = false;
    //    }
    //}



    private void ExitGame()
    {
        // 종료버튼
        // 아니면 타이틀로?
        Application.Quit();
    }
}
