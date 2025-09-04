using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniSelection : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup group; // 선택지 전체 그룹
    [SerializeField] private Button yesBTN;
    [SerializeField] private Button noBTN;

    private bool standbyInput = false; // 사용자 입력

    // 선택값
    private int focusIdx = -1;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OccurYN>(OnMiniSelection);
        yesBTN.onClick.AddListener(() => OnChoice(true));
        noBTN.onClick.AddListener(() => OnChoice(false));
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OccurYN>(OnMiniSelection);
        yesBTN.onClick.RemoveAllListeners();
        noBTN.onClick.RemoveAllListeners();
    }

    private void OnMiniSelection(UIEvents.OccurYN evt) { 
        standbyInput = true;
        group.alpha = 1f;
    }

    private void OnChoice(bool YN) {
        if (!standbyInput) return;

        //EventBus.Instance.Publish<UIEvents.SelectYN>(new UIEvents.SelectYN(true));
    }
}
