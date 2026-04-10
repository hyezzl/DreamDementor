using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReactZone : TriggerZone
{
    [Header("State")]
    protected bool isActivate = false;


    /// <summary>
    /// 리액트함수에서 실행될 함수
    /// </summary>
    protected virtual void PushTrigger() {
        if (isActivate) return;

        isActivate = true;
    }


    // 실행부
    public override void OnTrigger(GameObject actor)
    {
        // npcID값이 완료이벤트에 있으면 동작 / 없으면 무시

        if (isActivate)
        {
            base.OnTrigger(actor);
        }
        else
        {
            Debug.Log("트리거 무시!");
        }
    }
}
