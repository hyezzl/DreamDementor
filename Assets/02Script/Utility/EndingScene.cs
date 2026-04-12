using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 3f;

    [Header("UI Ref")]
    [SerializeField] private CanvasGroup endingScene;
    [SerializeField] private CanvasGroup black;
    [SerializeField] private Image illust;
    [SerializeField] private TextMeshProUGUI overText;
    [SerializeField] private CanvasGroup overGroup;

    // 버튼
    [SerializeField] private Button titleBTN;
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

        EventBus.Instance.Subscribe<GameEvents.OpenEndingScene>(ToEnd);
        titleBTN.onClick.AddListener(ToTitleScene);
        exitBTN.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OpenEndingScene>(ToEnd);
        titleBTN.onClick.RemoveListener(ToTitleScene);
        exitBTN.onClick.RemoveListener(ExitGame);
    }


    private void ToEnd(GameEvents.OpenEndingScene evt)
    {
        // 게임모드
        PlayerController.Instance.CurMode = GameMode.GameOverMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));

        // 엔딩씬 연출
        StopAllCoroutines(); // 혹시 실행 중인 다른 연출이 있다면 중지
        StartCoroutine(DeadSceneCor());
    }

    private IEnumerator DeadSceneCor()
    {
        // 효과음
        endingScene.alpha = 1f;

        // 검은배경 페이드인
        //yield return StartCoroutine(Fade(black, 0f, 1f, 1f));

        black.gameObject.SetActive(true);
        black.alpha = 1f;   // 페이드인보다는 바로 까매지는게 나은 듯

        yield return new WaitForSeconds(1f);

        // fade아웃 직전에 일러스트 켜줌
        illust.gameObject.SetActive(true);

        // 검은배경 페이드아웃
        yield return StartCoroutine(Fade(black, 1f, 0f, 3f));

        yield return new WaitForSeconds(5f);        // 2초동안 일러스트 보여줌

        // 검은배경 페이드인
        yield return StartCoroutine(Fade(black, 0f, 0.8f, 2f));

        // 글자 페이드인 // 3
        //overGroup.gameObject.SetActive(true);
        //yield return StartCoroutine(Fade(overGroup, 0f, 1f, 3f));

        // 마우스 생성
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        // 버튼 활성화
        BTNGroup.gameObject.SetActive(true);
        StartCoroutine(Fade(BTNGroup, 0f, 1f, 1f));
        BTNGroup.interactable = true;
    }

    private IEnumerator AfterClick()
    {
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


    private void ExitGame()
    {
        Debug.Log("게임이 종료 됩니다!!!!!!!!!!!!!!!!!!!!!!!!!");

        // 종료버튼
        EventBus.Instance.Publish<UIEvents.WarningShutdown>(new UIEvents.WarningShutdown());
    }

    private void ToTitleScene()
    {
        // 버튼을 한 번 눌렀으면 더 이상 클릭되지 않게 막음
        BTNGroup.interactable = false;

        // AfterClick 연출을 보고 싶다면 코루틴으로 실행 후 로드
        StartCoroutine(LoadTitleWithFade());
    }

    private IEnumerator LoadTitleWithFade()
    {
        yield return StartCoroutine(AfterClick());
        SceneManager.LoadScene("TitleScene");
    }
}
