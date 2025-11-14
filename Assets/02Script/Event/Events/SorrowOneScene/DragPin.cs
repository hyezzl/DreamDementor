using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPin : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int pinIdx;      // 해당 핀 인덱스
    public float targetY;       // 정답 Y값
    public Sprite answerImg;     // 정답 시 바뀔 스프라이트

    private RectTransform pin;          // 해당 핀
    private Image pinImg;
    private float allowDiff = 10f;     // 허용 오차
    private Vector2 originPos;          // 기존 위치
    private bool isFixed = false;       // 정답처리되어 고정되었는지
    private bool isDrag = false;        // 드래그 중인지
    private float holdTime = 0f;        // 정답근처에 머무른 시간
    private float requireTime = 0.5f;   // 정답처리 시간

    private float dragOffset;         // 포인터와 핀위치 차이 (Y)

    public bool IsFixed => isFixed;


    private void Start()
    {
        pin = GetComponent<RectTransform>();
        pinImg = GetComponent<Image>();
        originPos = pin.anchoredPosition;
    }


    private void Update()
    {
        CheckY();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isFixed) return;

        // 초기화
        isDrag = true;
        holdTime = 0f;
        originPos = pin.anchoredPosition;

        // 포인터 위치 UI좌표 변환
        Vector2 pointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pin.parent as RectTransform,
            eventData.position,
            null,
            out pointerPos);

        // 마우스와 핀 위치간 차이 계산 후 offset저장
        dragOffset = pin.anchoredPosition.y - pointerPos.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFixed) return;
        if (!isDrag) return;

        // 드래그 중 Y값 업데이트
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pin.parent as RectTransform,
            eventData.position,
            null,
            out localPos);

        //pin.anchoredPosition = new Vector2(pin.anchoredPosition.x, localPos.y);
        pin.anchoredPosition = new Vector2(pin.anchoredPosition.x, localPos.y + dragOffset);
        //pin.anchoredPosition = localPos + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isFixed) return;

        ResetPin();
        isDrag = false;
        holdTime = 0f;
    }


    // 정답 체크
    public void CheckY() {
        if (isFixed) return;
        if (!isDrag) return;

        // Y 값 비교
        if (Mathf.Abs(pin.anchoredPosition.y - targetY) <= allowDiff)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= requireTime)
            {
                FixPin();

                EventBus.Instance.Publish<PuzzleEvents.SO_FixPin>(new PuzzleEvents.SO_FixPin());
            }
        }
        else {
            // 정답 범위에서 한번이라도 벗어나면 다시 holdTime 초기화
            holdTime = 0f;
        }

    }

    // 정답 처리 함수
    public void FixPin() {
        isFixed = true;

        pin.anchoredPosition = new Vector2(pin.anchoredPosition.x, targetY);
        if (answerImg != null && pinImg != null) {
            pinImg.sprite = answerImg;
        }

        isDrag = false;
        holdTime = 0f;
    }

    // 오답 + 클릭 풀릴 시
    public void ResetPin() {
        // 변수
        isFixed = false;
        holdTime = 0f;
        isDrag = false;

        // 애니메이션처럼 느리게..
        //pin.anchoredPosition = originPos;
        pin.DOAnchorPos(originPos, 0.5f).SetEase(Ease.OutExpo);
    }
}
