using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FullIllustration : MonoBehaviour
{
    [Header("UIRefs")]
    [SerializeField] private CanvasGroup fullScene;
    [SerializeField] private Image img;

    private float duration = 1f;    // 페이드인/아웃 간격
    private AsyncOperationHandle<Sprite> handle;
    private bool loadingComplete = false;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.SceneCover>(OnSceneCover);
        EventBus.Instance.Subscribe<UIEvents.SceneDiscover>(OnSceneDiscover);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.SceneCover>(OnSceneCover);
        EventBus.Instance.Unsubscribe<UIEvents.SceneDiscover>(OnSceneDiscover);

        // 이전에 사용한 에셋 해제
        if (handle.IsValid()) {
            Addressables.Release(handle);
        }
    }

    private void OnSceneCover(UIEvents.SceneCover evt) {
        // 이미 로드된 에셋 해제
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }

        // 로드
        handle = Addressables.LoadAssetAsync<Sprite>(evt.illID);
        handle.Completed += OnLoaded;

        // 페이드인
        StartCoroutine(FadeIn(fullScene, duration));
    }

    private void OnLoaded(AsyncOperationHandle<Sprite> ill) {
        if (ill.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite fullIll = ill.Result;
            img.sprite = fullIll;
        }
        else {
            Debug.Log("풀 일러스트 로드 실패");
        }
        loadingComplete = true;
    }

    // 풀씬일러스트 닫힐 때
    private void OnSceneDiscover(UIEvents.SceneDiscover evt) {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }

        StartCoroutine(FadeOut(fullScene, duration));
    }

    public IEnumerator FadeIn(CanvasGroup group, float duration) {
        float elapse = 0f;
        fullScene.alpha = 0f;

        while (elapse < duration) { 
            elapse += Time.deltaTime;
            fullScene.alpha = Mathf.Clamp01(elapse / duration);
            yield return null;
        }
        group.alpha = 1f;
    }

    public IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        float elapse = 0f;
        fullScene.alpha = 1f;

        while (elapse < duration)
        {
            elapse += Time.deltaTime;
            group.alpha = 1 - Mathf.Clamp01(elapse / duration);
            yield return null;
        }
        fullScene.alpha = 0f;
    }


    // 줌인 함수
    public IEnumerator ZoomInAtPoint(RectTransform rt, Vector2 pivot, Vector3 targetPosition, float targetScale, float duration)
    {
        Vector3 initialScale = rt.localScale;
        Vector2 initialPivot = rt.pivot;
        Vector2 initialPos = rt.anchoredPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rt.pivot = Vector2.Lerp(initialPivot, pivot, t);
            rt.anchoredPosition = Vector2.Lerp(initialPos, targetPosition, t);
            rt.localScale = Vector3.Lerp(initialScale, Vector3.one * targetScale, t);

            yield return null;
        }

        rt.pivot = pivot;
        rt.anchoredPosition = targetPosition;
        rt.localScale = Vector3.one * targetScale;
    }


}
