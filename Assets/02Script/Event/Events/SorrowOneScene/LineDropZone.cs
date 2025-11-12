using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 드롭영역 순서 R - P - B - Y
//              0 - 1 - 2 - 3

public enum LineColor
{ 
    Red,
    Purple,
    Blue,
    Yellow,
}

public class LineDropZone : MonoBehaviour, IDropHandler
{
    public LineColor color;     // 드롭영역의 색
    public GameObject answer;       // 정답일 때 보일 전선

    private bool isConnect;     // 연결 완료되었는지

    public bool IsConnect => isConnect;

    private void Start()
    {
        //초기화
        answer.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isConnect) return;

        var line = eventData.pointerDrag.GetComponent<LineStartPoint>();
        if (line != null) {
            if (line.color == color)
            {
                // 색이 동일하다면 정답보임
                answer.SetActive(true);
                line.FixLine();

                isConnect = true;

                EventBus.Instance.Publish<PuzzleEvents.SO_ConnectLine>(new PuzzleEvents.SO_ConnectLine(color));
            }
            else
            {
                // 오답이면 복구
                line.ReturnLine();
            }
        }
    }
}
