using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class H_FloorToFloor : EventReactBlockZone
{
    public HorrorFloor curFloor;
    private HorrorFloor unlockedFloor;        // 해금된 층 정보 (2중으로 검증된 정보)

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Instance.Subscribe<PuzzleEvents.H_GetPassword>(OnGetPasswordEvent);
        EventBus.Instance.Subscribe<PuzzleEvents.H_Unlock>(OnUnlock);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.H_GetPassword>(OnGetPasswordEvent);
        EventBus.Instance.Unsubscribe<PuzzleEvents.H_Unlock>(OnUnlock);
    }



    private void OnGetPasswordEvent(PuzzleEvents.H_GetPassword evt)
    {
        if (curFloor == evt.floor)
        {
            unlockedFloor = evt.floor; // 층 정보 저장
            PushTrigger();            
        }
    }

    protected override void ReadyToPass()
    {
        // 퍼즐이 풀리지 않았으면 계속 UI 띄움
        OpenUIPuzzle(unlockedFloor);
    }

    private void OnUnlock(PuzzleEvents.H_Unlock evt) {
        if (evt.floor == curFloor) {
            // 퍼즐이 해제됨
            EndBlock();
        }
    }

    private void EndBlock() {
        // 퍼즐이 해제되었으므로 오브젝트 비활성화
        EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
        gameObject.SetActive(false);
    }


    private void OpenUIPuzzle(HorrorFloor floor) {
        Debug.Log($"지금 상호작용되는 오브젝트이름 : {gameObject.name}");
        if (floor == HorrorFloor.Floor1)
        {
            // 1층 자물쇠
            EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(PuzzleType.Horror_Lock_1F));
        }

        else if (floor == HorrorFloor.Floor2) {
            // 2층 자물쇠
            EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(PuzzleType.Horror_Lock_2F));
        }
    }

    
}
