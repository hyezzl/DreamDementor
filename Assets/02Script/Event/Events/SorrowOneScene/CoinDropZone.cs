using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinDropZone : MonoBehaviour
{
    private int curCnt = 0;     // 현재 넣어진 동전의 개수
    private int completeCnt = 2;

    private RectTransform rect;



    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void HandleCoin(DropCoin coin) {

        curCnt++;
        Debug.Log("코인 들어감!");

        //코인 정답 처리
        coin.InsertCoin(transform.position);

        // 2개가 모두 들어갔으면 이벤트 실행
        if (curCnt >= completeCnt) {
            EventBus.Instance.Publish<PuzzleEvents.SO_WorkVendingMachine>(new PuzzleEvents.SO_WorkVendingMachine());
        }
    }
}
