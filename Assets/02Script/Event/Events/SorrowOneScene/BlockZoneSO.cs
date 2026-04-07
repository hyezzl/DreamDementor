using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockZoneSO : BlockTriggerZone
{
    public bool isOk = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        EventBus.Instance.Subscribe<PuzzleEvents.SO_GetAllNumber>(ReadyToOpen);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Instance.Unsubscribe<PuzzleEvents.SO_GetAllNumber>(ReadyToOpen);
    }


    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsSatisfying())
            {
                // 블락이벤트
                EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
                EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
            }
            else
            {
                // zone 상태 저장
                EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));

                // 2. 물리적 비활성화 (플레이어가 슥 지나가게 함)
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }

    // 나갈 기준 만족했는지
    private bool IsSatisfying()
    {
        return isOk;
    }

    private void ReadyToOpen(PuzzleEvents.SO_GetAllNumber evt) { 
        isOk = true;
    }
}
