using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 일러스트 감상 시, 스파인 비활성화
/// </summary>

public class SpineManager : MonoBehaviour
{
    [SerializeField] private GameObject spine;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.SceneCover>(OnCover);
        EventBus.Instance.Subscribe<UIEvents.SceneDiscover>(OnDiscover);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.SceneCover>(OnCover);
        EventBus.Instance.Unsubscribe<UIEvents.SceneDiscover>(OnDiscover);
    }

    // 일러스트 등장
    private void OnCover(UIEvents.SceneCover evt) {
        spine.SetActive(false);
    }

    private void OnDiscover(UIEvents.SceneDiscover evt) {
        spine.SetActive(true);
    }
}
