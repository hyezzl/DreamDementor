using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class preDeadScene : MonoBehaviour
{
    [SerializeField] private GameObject topBack;
    [SerializeField] private GameObject bottomBack;
    [SerializeField] private Image bloodSpot1;
    [SerializeField] private Image bloodSpot2;
    [SerializeField] private float fadeDuration = 5f;

    // 버튼
    [SerializeField] private Button loadBTN;
    [SerializeField] private Image backImg;
    [SerializeField] private CanvasGroup loadGroup;

    [SerializeField] private Button exitBTN;
    [SerializeField] private Image backImg2;
    [SerializeField] private CanvasGroup exitGroup;


    //  현재 죽음에 대한 정보 캐싱값
    private SceneType scene;
    private DeathType deathType;


    private void Start()
    {
        topBack.SetActive(false);
        bottomBack.SetActive(false);

        bloodSpot1.enabled = false;
        bloodSpot2.enabled = false;

        backImg.enabled = false;
        loadBTN.gameObject.SetActive(false);        // 버튼 비활성화 상태로 시작

        backImg2.enabled = false;
        exitBTN.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.TutorialDead>(OnOver);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.TutorialDead>(OnOver);
    }

    private void OnOver(GameEvents.TutorialDead evt)
    {
        // 카메라 필터

        // 배경
        topBack.SetActive(true);
        bottomBack.SetActive(true);

        // 게임모드
        PlayerController.Instance.CurMode = GameMode.GameOverMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.GameOverMode));

        // 게임오버 정보 캐싱
        scene = evt.scene;
        deathType = evt.type;

        StartCoroutine(AfterDead()); // 데드씬 연출 + 이후 처리

    }

    private IEnumerator BloodEffect()
    {
        yield return null;
        float elapsed = 0f;

        bloodSpot1.enabled = true;
        //효과음

        yield return new WaitForSeconds(0.5f);

        bloodSpot2.enabled = true;
        //효과음

        float startAlpha1 = bloodSpot1.color.a;
        float startAlpha2 = bloodSpot2.color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha1 = Mathf.Lerp(startAlpha1, 0f, elapsed / fadeDuration);
            float alpha2 = Mathf.Lerp(startAlpha2, 0f, elapsed / fadeDuration);

            bloodSpot1.color = new Color(bloodSpot1.color.r, bloodSpot1.color.g, bloodSpot1.color.b, alpha1);
            bloodSpot2.color = new Color(bloodSpot2.color.r, bloodSpot2.color.g, bloodSpot2.color.b, alpha2);
            yield return null;
        }
        bloodSpot1.color = new Color(bloodSpot1.color.r, bloodSpot1.color.g, bloodSpot1.color.b, 0f);
        bloodSpot2.color = new Color(bloodSpot2.color.r, bloodSpot2.color.g, bloodSpot2.color.b, 0f);
    }

    // 데드씬 끝난 이후 처리
    private IEnumerator AfterDead()
    {
        yield return StartCoroutine(BloodEffect());  // 코루틴 끝날 때 까지 기다림

        if (scene == SceneType.TutorialScene && deathType == DeathType.CrashEnemy) // D0100
        {
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent("E004"));
            yield break;
        }
    }




    // 버튼 생성 함수
    private IEnumerator ShowLoadButton()
    {
        loadBTN.gameObject.SetActive(true);
        exitBTN.gameObject.SetActive(true);

        float elapsed = 0f;
        float duration = 1.5f;      // 페이드인

        Color imgColor = backImg.color;     // 기존컬러 캐싱
        imgColor.a = 0f;
        backImg.color = imgColor;

        loadGroup.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);

            imgColor.a = alpha;
            backImg.color = imgColor;

            loadGroup.alpha = alpha;

            yield return null;
        }
        imgColor.a = 1f;
        backImg.color = imgColor;
        loadGroup.alpha = 1f;
        exitGroup.alpha = 1f;
    }



    /// 버튼

    // loadBTN 전용
    public void OnLoadButtonPointerEnter(PointerEventData eventData)
    {
        if (backImg != null)
            backImg.enabled = true;
    }
    public void OnLoadButtonPointerExit(PointerEventData eventData)
    {
        if (backImg != null)
            backImg.enabled = false;
    }

    // exitBTN 전용
    public void OnExitButtonPointerEnter(PointerEventData eventData)
    {
        if (backImg2 != null)
            backImg2.enabled = true;
    }
    public void OnExitButtonPointerExit(PointerEventData eventData)
    {
        if (backImg2 != null)
            backImg2.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == loadBTN.gameObject)
        {
            if (backImg != null) backImg.enabled = true;
        }
        else if (eventData.pointerEnter == exitBTN.gameObject)
        {
            if (backImg2 != null) backImg2.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter == loadBTN.gameObject)
        {
            if (backImg != null) backImg.enabled = false;
        }
        else if (eventData.pointerEnter == exitBTN.gameObject)
        {
            if (backImg2 != null) backImg2.enabled = false;
        }
    }
}
