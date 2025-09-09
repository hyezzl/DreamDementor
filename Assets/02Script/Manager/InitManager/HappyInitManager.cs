using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyInitManager : InitManager
{
    [Header("HappyScene 내 스크립트")]
    [SerializeField] private TeleportHappyScene teleport;

    protected override void Start()
    {
        base.Start();

        // Happy Scene
        // 강제 이동 후 즉발
        if (teleport != null) {
            teleport?.Init(db);
        }
        
    }
}
