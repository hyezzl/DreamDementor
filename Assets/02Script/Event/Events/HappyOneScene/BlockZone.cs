using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockZone : BlockTriggerZone
{

    private int[] requireIDs = { 10001002, 10001003, 10001004 };


    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { 
            if (!IsSatisfying())
            {
                Debug.Log("열쇠를 가지고 있지 않음");

                EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
                EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
            }
            else {
                Debug.Log("열쇠를 모두 가지고계십니다.");
            }
        }
    }


    // 열쇠 3개가 모두 있는지 
    private bool IsSatisfying()
    {
        // 인벤토리 가져옴
        var inventory = InventoryManager.Instance.GetInventory();

        foreach (var id in requireIDs)
        {
            bool hasKey = false;
            foreach (var item in inventory)
            {
                if (item.itemID == id)
                {
                    hasKey = true;
                    break;
                }
            }
            if (!hasKey)
                return false;   // 하나라도 없으면 false
        }
        return true;
    }
}
