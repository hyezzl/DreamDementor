using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainUI : CameraManager
{

    [SerializeField] CinemachineVirtualCamera cam;
    protected override void Awake()
    {
        base.Awake();
        SelectCam(cam, 0);
    }
}
