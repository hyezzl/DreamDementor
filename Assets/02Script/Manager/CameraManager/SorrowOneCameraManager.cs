using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorrowOneCameraManager : CameraManager
{
    [SerializeField] private CinemachineVirtualCamera fixCam;
    [SerializeField] private CinemachineVirtualCamera enemyCam;
    protected override void Awake()
    {
            base.Awake();
            cams = new List<CinemachineVirtualCamera> { fixCam, enemyCam };
    }
    private void Start()
    {
        SelectCam(fixCam, 0);
    }

    protected override void OnCameraShift(GameEvents.CameraShift evt)
    {
        switch (evt.type)
        {
            case CameraType.PlayerSightCam:
                SelectCam(fixCam, evt.blend);
                break;

            case CameraType.EnemyCam:
                SelectCam(enemyCam, evt.blend);
                break;
        }
    }
}

