using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinDropZone : MonoBehaviour, IDropHandler
{
    private int curCnt = 0;     // 현재 넣어진 동전의 개수

    private RectTransform rect;



    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        var coin = eventData.pointerDrag?.GetComponent<DropCoin>();
        if (coin != null) { 
            curCnt++;

            // 드래그 지점이 드롭존 범위 안인지 체크
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, null))
            {
                // 드랍존 위치에서 애니메이션 실행
                coin.transform.position = transform.position;

                // 애니메이션 실행

                EventBus.Instance.Publish<PuzzleEvents.SO_InsertCoin>(new PuzzleEvents.SO_InsertCoin());

                if (curCnt == 2)
                {

                }
            }
            else { 
                // 드랍존 외 드랍 시
            }

        }

    }
}
