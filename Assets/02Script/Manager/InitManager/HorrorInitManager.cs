using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorInitManager : InitManager
{
    private WindowPuzzle windowP;

    protected override void Start()
    {
        base.Start();

        windowP = FindAnyObjectByType<WindowPuzzle>();
        if (windowP == null) Debug.Log("SorrowOneInitManager - Failed to Load WindowEventData");

        windowP.Init(db);
    }
}
