using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup group;

    public float fadeDuration = 2f;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.OnDamaged>(OnDamaged);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.OnDamaged>(OnDamaged);
    }

    private void OnDamaged(GameEvents.OnDamaged evt) {
        // 데미지 입으면 등장 
        StopAllCoroutines();
        group.alpha = 1f;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {
        float time = 0f;
        float start = group.alpha;

        while (time < fadeDuration) {
            group.alpha = Mathf.Lerp(start, 0f, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        group.alpha = 0f;
    }
}
