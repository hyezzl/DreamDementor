using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Selection : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image background;  // 비활성화 효과 배경
    [SerializeField] private GameObject choiceBTNprefab;  // 버튼 프리팹
    [SerializeField] private Transform BTNarea;   // 버튼 영역(부모)

    [SerializeField] private GridLayoutGroup group;

    

    private bool standbyInput = false;
    private List<GameObject> buttons = new();
    private string curChoiceID = null;

    // 선택
    private int focusIdx = -1;
    private Tween focusTween;


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OccurSelection>(OnSelection);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OccurSelection>(OnSelection);
    }


    private void Update()
    {
        if (!standbyInput) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int nextIdx = focusIdx + 1;
            //if (nextIdx >= buttons.Count) nextIdx = 0;
            SetFocus(nextIdx);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int preIdx = focusIdx - 1;
            //if (preIdx < 0) preIdx = buttons.Count - 1;
            SetFocus(preIdx);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // 모바일에서는...상호작용키보다 클릭이겠죠..?
        {
            OnChoice(focusIdx);
        }
    }


    private void OnSelection(UIEvents.OccurSelection evt) {
        standbyInput = true;
        background.enabled = true;

        if (evt.choice.texts.Count == 2)  // 선택지 2개일 때
        {
            Vector2 spacing = group.spacing;
            spacing.x = 700f;
            group.spacing = spacing;
        }
        else {
            Vector2 spacing = group.spacing;
            spacing.x = 20f;
            group.spacing = spacing;
        }
            CreateBTN(evt.idx, evt.choice);
    }

    // 버튼 개수대로 생성
    private void CreateBTN(int num, ChoiceData data) {
        curChoiceID = data.choiceID; // 캐싱
        // 기존 버튼 비활성화
        foreach (var btn in buttons) {
            btn.SetActive(false);
        }

        for (int i = 0; i < num; i++) {
            GameObject btn;

            if (i < buttons.Count)
                btn = buttons[i];
            else {
                btn = Instantiate(choiceBTNprefab, BTNarea);
                buttons.Add(btn);
            }
            btn.SetActive(true);

            // 버튼 텍스트
            btn.GetComponentInChildren<TextMeshProUGUI>().text = data.texts[i];
            btn.transform.Find("focusbox").GetComponent<Image>().enabled = false; // 포커스 비활성화

            // 버튼 클릭 이벤트 연결 (예: i번째 선택지를 넘기는 커스텀 함수에 연결)
            btn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();  // 초기화
            int choiceIndex = i;
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoice(choiceIndex));
        }
        SetFocus(0);
    }


    private void SetFocus(int index) {
        if (buttons.Count == 0) return;

        // clamp
        index = Mathf.Clamp(index, 0, buttons.Count - 1);
        if (index == focusIdx) return; // 끝에 도달했을 경우 focus함수 호출X


        // 이전 포커스 박스 끄기 + 트윈 종료
        if (focusIdx >= 0 && focusIdx < buttons.Count)
        {
            Transform preFocus = buttons[focusIdx].transform.Find("focusbox");
            Image preFocusImg = preFocus.GetComponent<Image>();
            preFocusImg.enabled = false;

            if (focusTween != null && focusTween.IsActive()) {
                focusTween.Kill(); // 트윈 종료
            }
            preFocus.localScale = Vector3.one; // 원상태
        }

        // 인덱스 값
        focusIdx = index;

        // 현재 포커스 버튼 포커스 활성화
        Transform focusBox = buttons[focusIdx].transform.Find("focusbox");
        Image focusImg = focusBox.GetComponent<Image>();
        focusImg.enabled = true;

        // Scale Animated
        focusBox.localScale = Vector3.one;
        focusTween = focusBox.DOScale(1.05f, 1f)
                                .SetLoops(6, LoopType.Yoyo)
                                .SetEase(Ease.InOutSine);

        // 선택된 버튼에 포커스 설정
        buttons[focusIdx].GetComponent<UnityEngine.UI.Button>().Select();
    }


    private void OnChoice(int idx) {
        if (!standbyInput) return;

        Debug.Log($"{idx} 번 선택됨!!!!!!!!!!!!!!!!");
        EventBus.Instance.Publish<UIEvents.MakeChoice>(new UIEvents.MakeChoice(curChoiceID, idx));

        // 버튼 비활성화
        foreach (var btn in buttons) {
            btn.SetActive(false);
        }

        background.enabled = false;
        curChoiceID = null;
        standbyInput = false;
    }
}
