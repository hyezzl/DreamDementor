using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCameraManager : CameraManager
{
    [SerializeField] private CinemachineVirtualCamera fixCam;
    [SerializeField] private CinemachineVirtualCamera enemyCam;
    [SerializeField] private CinemachineVirtualCamera betweenCam;

    protected override void Awake(){ 
        base.Awake();
        cams = new List<CinemachineVirtualCamera> { fixCam, enemyCam, betweenCam };
    }

    private void Start()
    {
        SelectCam(fixCam, 0);
    }

    protected override void OnCameraShift(GameEvents.CameraShift evt)
    {
        switch (evt.type)
        {
            case CameraType.PlayerFixCam:
                SelectCam(fixCam, evt.blend);
                break;

            case CameraType.EnemyCam:
                SelectCam(enemyCam, evt.blend);
                break;

            case CameraType.BetweenCam:
                SelectCam(betweenCam, evt.blend);
                break;
        }
    }
}