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

    public GameObject GetObject() { return this.gameObject; }
}
