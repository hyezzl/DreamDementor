using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image highlight;              // 버튼 하이라이트 이미지
    [SerializeField] private TitleBTN[] otherButtons;   // 같은 그룹의 버튼들
    private bool isSelected = false;

    private void Awake()
    {
        if (highlight != null)
            highlight.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && highlight != null)
            highlight.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && highlight != null)
            highlight.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 시 자신만 활성화 + 나머지는 비활성화
        foreach (var btn in otherButtons)
        {
            btn.Deselect();
        }

        Select();

        // 버튼별 행동 정의
        if (name.Contains("Start"))
        {
            Debug.Log("게임 시작!");
            EventBus.Instance.Publish<GameEvents.SwitchScene>(
                new GameEvents.SwitchScene(SceneType.TutorialScene));
        }
        else if (name.Contains("Load"))
        {
            Debug.Log("불러오기 버튼 클릭!");
        }
        else if (name.Contains("Exit"))
        {
            Debug.Log("게임 종료!");
            Application.Quit();
        }
    }

    private void Select()
    {
        isSelected = true;
        if (highlight != null)
            highlight.enabled = true;
    }

    public void Deselect()
    {
        isSelected = false;
        if (highlight != null)
            highlight.enabled = false;
    }
}
