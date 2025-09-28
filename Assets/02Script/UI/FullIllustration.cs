using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        StartCoroutine(FadeIn());
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
    }

    // 풀씬일러스트 닫힐 때
    private void OnSceneDiscover(UIEvents.SceneDiscover evt) {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }

        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn() {
        float elapse = 0f;
        fullScene.alpha = 0f;

        while (elapse < duration) { 
            elapse += Time.deltaTime;
            fullScene.alpha = Mathf.Clamp01(elapse / duration);
            yield return null;
        }
        fullScene.alpha = 1f;
    }

    public IEnumerator FadeOut()
    {
        float elapse = 0f;
        fullScene.alpha = 1f;

        while (elapse < duration)
        {
            elapse += Time.deltaTime;
            fullScene.alpha = 1 - Mathf.Clamp01(elapse / duration);
            yield return null;
        }
        fullScene.alpha = 0f;
    }


}
