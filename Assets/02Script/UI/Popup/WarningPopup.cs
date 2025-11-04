using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 게임을 종료하려고 할 때 재 확인
/// </summary>
public class WarningPopup : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private GameObject window;     // 경고창
    [SerializeField] private Button yesBTN;
    [SerializeField] private Button noBTN;

    private void OnEnable()
    {
        window.SetActive(false);

        EventBus.Instance.Subscribe<UIEvents.WarningShutdown>(OnShutdown);

        yesBTN.onClick.AddListener(ShutdownGame);
        noBTN.onClick.AddListener(Cancel);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.WarningShutdown>(OnShutdown);

        yesBTN.onClick.RemoveListener(ShutdownGame);
        noBTN.onClick.RemoveListener(Cancel);
    }

    // 게임종료 이벤트 넘어오면
    private void OnShutdown(UIEvents.WarningShutdown evt) {
        // 게임종료 재질문 창 켜짐
        window.SetActive(true);
    }

    // 게임 종료
    private void ShutdownGame() { 
        Application.Quit();
    }

    // 게임종료 취소 (창 다시 끔)
    private void Cancel() { 
        window.SetActive(false);
    }
}
