using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;

/// <summary>
///  씬전환 시 카메라필터를 사용한 효과 (메인카메라 컴포넌트)
/// </summary>
/// 
[RequireComponent(typeof(CameraFilterPack_FX_Spot))]
public class SwitchSceneEffect : MonoBehaviour
{
    [Header("Effect Setting")]
    [SerializeField] private float fadeDuration = 2.5f;
    [SerializeField] private float fadeInVal = 1f;
    [SerializeField] private float fadeOutVal = -0.15f;

    private CameraFilterPack_FX_Spot spot;
    private SceneType curScene = SceneType.None;

    private void Awake()
    {
        if (!TryGetComponent<CameraFilterPack_FX_Spot>(out spot)) Debug.Log("SwitchSceneEffect - Failed to Load SpotFilter");

        // 초기값
        spot.Radius = fadeInVal;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.SceneStartEffect>(SceneStarted);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.SceneStartEffect>(SceneStarted);
    }

    private void SceneStarted(GameEvents.SceneStartEffect evt) {
        curScene = evt.scene;

        StartCoroutine(FadeInScene());
    }

    // 씬 열릴 때 효과 (일반)
    private IEnumerator FadeInScene() {
        if (spot == null) yield break;

        float elapsed = 0f;
        spot.Radius = fadeOutVal;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            spot.Radius = Mathf.Lerp(fadeOutVal, fadeInVal, t);

            yield return null;
        }
        spot.Radius = fadeInVal;

        // 효과가 모두 끝나고 실행
        if (curScene != SceneType.None) { 
            EventBus.Instance.Publish<GameEvents.SceneEffectEnd>(new GameEvents.SceneEffectEnd(curScene));
        }
    }
}
