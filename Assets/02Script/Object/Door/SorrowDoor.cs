using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowDoor : NPC
{
    public string checkEventID = "E200";

    public override void Interact()
    {
        if (isInDialog || isDelay) return;

        // 조건 이벤트 만족했는지 검사
        bool hasAllNum = EventHistoryManager.Instance.IsEventComplete(checkEventID);

        Debug.Log($" !!!!재방문여부 :: {EventHistoryManager.Instance.IsEventComplete(checkEventID)}");
        if (hasAllNum)
        {
            // 재방문일때 UI 엶
            Debug.Log("재방문임이 판명되었습니다..");
            EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(PuzzleType.Sorrow_Door));
        }
        else
        {
            // 첫방문
            Debug.Log("첫방문 로직");
            base.Interact();
        }
    }

    
}
