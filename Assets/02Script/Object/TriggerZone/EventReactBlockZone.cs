using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReactBlockZone : EventReactZone
{
    protected PlayerMove pm;

    protected override void Awake()
    {
        base.Awake();
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerMove");
    }

    /// <summary>
    /// 조건을 달성했을 때 실행할 함수 (자식)
    /// </summary>
    protected virtual void ReadyToPass() { }






    public override void OnTrigger(GameObject actor)
    {
        if (!isActivate)
        {
            // "못 지나간다"
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
        else
        {
            // 통과가능 (바로 해제하지않고, 조건부 해제)
            //EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
            //gameObject.SetActive(false);

            ReadyToPass();
        }
    }
    public override void OnEndDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == eventID)
        {
            if (!isActivate)
            {
                pm.PushBack();
            }
        }
    }
}
