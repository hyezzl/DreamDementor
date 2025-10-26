using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 드래그 가능한 아이템에 붙이는 컴포넌트
/// (마우스로 드래그&드롭이 가능)
/// </summary>


[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Canvas canvas;
    public PuzzleItem itemName;
    private RectTransform rect;
    private CanvasGroup group;
    private Vector2 originPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        originPos = rect.anchoredPosition;
    }


    // 드래그 시작 시 호출
    public void OnBeginDrag(PointerEventData evt)
    {
        group.alpha = 0.7f;     // 클릭했을때 투명
        group.blocksRaycasts = false;
    }


    // 드래그 중
    public void OnDrag(PointerEventData evt)
    {
        rect.anchoredPosition += evt.delta / canvas.scaleFactor;    // 캔버스 대비 변화값
    }

    // 드래그 끝
    public void OnEndDrag(PointerEventData evt)
    {
        group.alpha = 1f;
        group.blocksRaycasts = true;

        // 드랍이 실패
        if (evt.pointerEnter == null || !evt.pointerEnter.CompareTag("PuzzleTarget")) { 
            rect.anchoredPosition = originPos;
        }
    }
}
