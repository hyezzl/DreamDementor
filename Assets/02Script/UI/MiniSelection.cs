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
    [SerializeField] private Image yesFocus;
    [SerializeField] private Image noFocus;

    private bool standbyInput = false; // 사용자 입력

    // 선택값
    private int focusIdx = -1;
    private List<Button> buttons;

    private void Awake()
    {
        buttons = new List<Button> { yesBTN, noBTN };
    }

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
        group.blocksRaycasts = true;
        group.interactable = true;

        yesFocus.enabled = false;
        noFocus.enabled = false;

        SetFocus(0);
    }


    private void Update()
    {
        if (!standbyInput) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            int nextIdx = focusIdx + 1;
            SetFocus(nextIdx);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            int preIdx = focusIdx - 1;
            SetFocus(preIdx);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            if (focusIdx == 0) OnChoice(true);
            else if(focusIdx == 1) OnChoice(false);
        }
    }

    private void SetFocus(int idx) {
        if (idx == focusIdx) return;
        idx = Mathf.Clamp(idx, 0, 1);

        // 이전 포커스 끄기
        if (focusIdx >= 0 && focusIdx < 2) {
            Button preFocus = buttons[focusIdx];
            Image preFocusImg = focusIdx == 0 ? yesFocus : noFocus;
            preFocusImg.enabled = false;
        }

        focusIdx = idx;

        // 현재 포커스 활성화
        Button focus = buttons[focusIdx];
        Image focusImg = focusIdx == 0 ? yesFocus : noFocus;
        focusImg.enabled = true;

        // 선택된 버튼에 포커스 설정
        buttons[focusIdx].GetComponent<UnityEngine.UI.Button>().Select();
    }

    private void OnChoice(bool YN) {
        if (!standbyInput) return;

        if (YN)
        {
            // todo : 추가 처리
            EventBus.Instance.Publish<UIEvents.SelectYN>(new UIEvents.SelectYN(true));
        }
        else {
            // todo : 추가 처리
            EventBus.Instance.Publish<UIEvents.SelectYN>(new UIEvents.SelectYN(false));
        }

        // 창 비활성화
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
        standbyInput = false;
    }
}
