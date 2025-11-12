using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LineStartPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public LineColor color;
    public RectTransform line;
    public Image tempImg;       // 짧은 임시 전선이미지


    private Vector2 startPos;
    private bool isFixed = false;       // 정답처리되어 고정되었는지


    private float originVer = 763f;
    private bool isDrag = false;

    private void Start()
    {
        // 초기화
        line.gameObject.SetActive(false);
        tempImg.enabled = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isFixed) return;
        isDrag = true;

        // 드래그 시작 시점에 정확한 UI 위치를 스크린 좌표로 얻기
        startPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);

        // 임시 전선 지우고 포인터따라 긴 전선 생성
        tempImg.enabled = false;
        line.gameObject.SetActive(true);
        UpdateLine(eventData.position);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFixed) return;
        if (!isDrag) return;

        UpdateLine(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isFixed) return;
        tempImg.enabled = true;
        line.gameObject.SetActive(false);
        isDrag = false;
    }

    // 선위치 실시간 업데이트
    private void UpdateLine(Vector2 curPos) {
        if (isFixed) return;

        line.position = startPos;

        float length = Vector2.Distance(startPos, curPos);

        // 기존것과 비교해 길이에 맞는 y스케일 조정
        Vector3 scale = line.localScale;
        scale.y = length / originVer;
        line.localScale = scale;

        // 방향 계산해서 회전
        Vector2 dir = curPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // atan2은 x축 기준이기 때문에, y축으로 늘어나려면 -90
        line.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }


    // 정답일 때
    public void FixLine() {
        isFixed = true;

        tempImg.enabled = false;
        line.gameObject.SetActive(false);
        isDrag = false;
    }

    // 오답일 때, 선 복구
    public void ReturnLine() {
        line.position = startPos;
        tempImg.enabled = true;
        line.gameObject.SetActive(false);
        isDrag = false;
    }
}
