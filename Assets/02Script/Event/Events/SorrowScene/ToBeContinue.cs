using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToBeContinue : MonoBehaviour
{
    [SerializeField] private CanvasGroup black;
    [SerializeField] private GameObject illust;
    //소리
    [SerializeField] private CanvasGroup tobeText;

    [SerializeField] private CanvasGroup exitButton;
    [SerializeField] private Button exitBTN;


    private void OnEnable()
    {
        exitBTN.onClick.AddListener(ExitGame);
    }
    private void OnDisable()
    {
        exitBTN.onClick.RemoveListener(ExitGame);
    }

    private void Awake()
    {
        exitBTN.interactable = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // 이벤트 모드
            PlayerController.Instance.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));

            // 브금끄기
            EventBus.Instance.Publish<GameEvents.StopBGM>(new GameEvents.StopBGM());

            StartCoroutine(OnTrigger());
        }
    }

    private IEnumerator OnTrigger() {

        // 검은배경 페이드인
        yield return StartCoroutine(Fade(black, 0f, 1f, 2f));

        yield return new WaitForSeconds(1f);

        // 소리 (6초)
        EventBus.Instance.Publish<GameEvents.PlaySFX>(new GameEvents.PlaySFX(SFXType.trafficAccident));
        yield return new WaitForSeconds(6f);

        // fade아웃 직전에 일러스트 켜줌
        illust.SetActive(true);

        // 검은배경 페이드아웃
        yield return StartCoroutine(Fade(black, 1f, 0f, 3f));

        yield return new WaitForSeconds(4f);        // 4초동안 일러스트 보여줌

        // 검은배경 페이드인
        yield return StartCoroutine(Fade(black, 0f, 1f, 2f));

        // 글자 페이드인 // 3
        yield return StartCoroutine(Fade(tobeText, 0f, 1f, 7f));

        // 마우스 생성
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        // 종료버튼 페이드인 + interactable On
        StartCoroutine(Fade(exitButton, 0f, 1f, 2f));
        exitBTN.interactable = true;


    }

    public IEnumerator Fade(CanvasGroup group, float startVal, float endVal, float fadeDuration) {
        float elapsed = 0f;

        while (elapsed < fadeDuration) { 
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startVal, endVal, elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = endVal;
    }

    private void ExitGame() {
        Application.Quit();
    }





}
