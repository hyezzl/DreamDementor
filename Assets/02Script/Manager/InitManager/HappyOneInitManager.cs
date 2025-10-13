using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyOneInitManager : InitManager
{
    [Header("HappyOneScene 내 스크립트")]
    [SerializeField] private HappyOne happyOne;

        protected override void Start()
    {
        base.Start();

        if (happyOne != null)
        {
            happyOne?.Init(db);
        }

    }
}
