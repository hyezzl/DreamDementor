using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockZoneSO : BlockTriggerZone
{
    // 통과 기준

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsSatisfying())
            {
                // 블락이벤트
                //EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
                //EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
            }
            else
            {
                Debug.Log("통과!.");
            }
        }
    }

    // 나갈 기준 만족했는지
    private bool IsSatisfying()
    {
        return false;
    }
}
