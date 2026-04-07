using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowOneInitManager : InitManager
{
    private SO_ChaseEnemy chase;

    protected override void Start()
    {
        base.Start();

        chase = FindAnyObjectByType<SO_ChaseEnemy>();
        if (chase == null) Debug.Log("SorrowOneInitManager - Failed to Load ChaseEventData");

        chase.Init(db);
    }
}
