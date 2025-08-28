using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopup : MonoBehaviour
{
    [SerializeField] private Image background;  // 비활성화 효과 배경
    [SerializeField] private GameObject choiceBTNprefab;  // 버튼 프리팹
    [SerializeField] private Transform BTNarea;   // 버튼 영역(부모)

    [SerializeField] private GridLayoutGroup group;

    

    private bool standbyInput = false;
    private List<GameObject> buttons = new();
    private string curChoiceID = null;


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.OccurSelection>(OnSelection);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.OccurSelection>(OnSelection);
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

            // 버튼 클릭 이벤트 연결 (예: i번째 선택지를 넘기는 커스텀 함수에 연결)
            btn.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            int choiceIndex = i;
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnChoice(choiceIndex));
        }
    }

    private void OnChoice(int idx) {
        if (!standbyInput) return;
        Debug.Log($"{idx} 번 선택됨!!!!!!!!!!!!!!!!");
        EventBus.Instance.Publish<GameEvents.MakeChoice>(new GameEvents.MakeChoice(curChoiceID, idx));

        // 버튼 비활성화
        foreach (var btn in buttons) {
            btn.SetActive(false);
        }

        background.enabled = false;
        curChoiceID = null;
        standbyInput = false;
    }

}
