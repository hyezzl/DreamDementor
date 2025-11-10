using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineStartPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform line;
    //public RectTransform startObject;

    private Vector2 startPos;

    private float originVer = 307f;
    private bool isDrag = false;


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;

        // 드래그 시작 시점에 정확한 UI 위치를 스크린 좌표로 얻기
        startPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);

        line.gameObject.SetActive(true);
        UpdateLine(eventData.position);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrag) return;

        UpdateLine(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        line.gameObject.SetActive(false);
        isDrag = false;
    }

    private void UpdateLine(Vector2 curPos) {
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
}
