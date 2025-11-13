using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomItem : MonoBehaviour, IActionItem
{
    public int itemID;

    public int GetItemID() => itemID;

    public ItemType GetItemType() { return ItemType.Custom; }

    public virtual void Init(IDatabase db) { }

    public virtual void Interact() { }

    public virtual void OnEnable() {
        EventBus.Instance.Subscribe<PuzzleEvents.CutOff>(OnCutoff);
    }

    public virtual void OnDisable() { 
        EventBus.Instance.Subscribe<PuzzleEvents.CutOff>(OnCutoff);
    }

    // 퍼즐 종료 후 tag Deactive로 변경
    public virtual void OnCutoff(PuzzleEvents.CutOff evt) {
        if (itemID == evt.itemID) {
            ChangeTag();
        }
    }


    public virtual void ChangeTag() {
        gameObject.tag = "Deactive";
    }


    public GameObject GetObject() { return this.gameObject; }
}
