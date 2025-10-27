using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyDoor : NPC
{

    private int[] requireIDs = { 10001002, 10001003, 10001004 };

    public override void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (IsSatisfying())
        {
            EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(SceneType.HappyScene));
        }
        // 아직 조건 만족하지 않음
        else { 
            if (!isContacted)
            {
                // 첫 대면 : 대화
                isInDialog = true;
                EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, npcID, initialDialog));
                curNpcEventID = npcID;
                isContacted = true;
            }
            else
            {
                // 재 대화시 재대화
                isInDialog = true;
                EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
            }
        }
    }



    // 열쇠 3개가 모두 있는지 
    private bool IsSatisfying() {
        // 인벤토리 가져옴
        var inventory = InventoryManager.Instance.GetInventory();

        foreach (var id in requireIDs) {
            bool hasKey = false;
            foreach (var item in inventory) {
                if (item.itemID == id) {
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
