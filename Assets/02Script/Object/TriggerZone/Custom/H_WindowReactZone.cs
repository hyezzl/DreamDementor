using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_WindowReactZone : EventReactZone
{
    public HorrorFloor curFloor;

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Instance.Subscribe<PuzzleEvents.H_GetPassword>(OnGetEvent);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.H_GetPassword>(OnGetEvent);
    }


    private void OnGetEvent(PuzzleEvents.H_GetPassword evt) {
        if (curFloor == evt.floor) {
            // 층이 같은 이벤트가 들어오면
            PushTrigger(); // 활성화
        }
    }
}
