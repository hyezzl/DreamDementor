using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstColumn : CustomItem
{
    [SerializeField] private Canvas minigame;
    [SerializeField] private RectTransform line2line;

    public override void Interact()
    {
        base.Interact();

        EventBus.Instance.Publish<UIEvents.OpenPuzzle>(new UIEvents.OpenPuzzle(PuzzleType.SorrowOne_LineToLine));
    }
}
