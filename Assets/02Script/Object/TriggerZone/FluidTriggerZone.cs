using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidTriggerZone : TriggerZone
{
    private EventHistoryManager hm;
    public string activeConditionID;

    protected override void Awake()
    {
        base.Awake();
        hm = FindAnyObjectByType<EventHistoryManager>();
        if (hm == null) Debug.Log($"FluidTriggerZone : {gameObject.name} - Failed to Load EventHistoryManager");
    }

    public override void OnTrigger(GameObject actor)
    {
        // npcID값이 완료이벤트에 있으면 동작 / 없으면 무시

        if (hm != null && hm.IsEventComplete(activeConditionID))
        {
            base.OnTrigger(actor);
        }
        else
        {
            Debug.Log("트리거 무시!");
        }
    }
}
