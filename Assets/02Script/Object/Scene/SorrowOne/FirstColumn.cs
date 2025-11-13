using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstColumn : CustomItem
{
    [SerializeField] private PuzzleType type;

    public override void Interact()
    {
        base.Interact();

        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(type));
    }
}
