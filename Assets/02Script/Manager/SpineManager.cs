using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SpineManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup spine;
    [SerializeField] private GameObject spine1;

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
        //spine.alpha = 0f;
        spine1.SetActive(false);
    }

    private void OnDiscover(UIEvents.SceneDiscover evt) {
        //spine.alpha = 1f;
        Debug.Log("여기안불려?");
        spine1.SetActive(true);
    }
}
