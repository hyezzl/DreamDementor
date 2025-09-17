using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyCameraManager : CameraManager
{
    [Header("Camera Refs")]
    [SerializeField] private CinemachineVirtualCamera fixCam;
    [SerializeField] private CinemachineVirtualCamera eyeeffect;

    protected override void Awake()
    {
        base.Awake();
        //if (PlayerController.Instance != null)
        //{
        //    GameObject player = PlayerController.Instance.gameObject;
        //    CinemachineVirtualCamera[] cameras = player.GetComponentsInChildren<CinemachineVirtualCamera>();

        //    if (cameras.Length > 0)
        //    {
        //        fixCam = cameras[0];
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("PlayerController.Instance is null!");
        //}

        cams = new List<CinemachineVirtualCamera> { fixCam, eyeeffect };

    }

    protected override void OnCameraShift(GameEvents.CameraShift evt)
    {
        switch (evt.type)
        {
            case CameraType.PlayerFixCam:
                SelectCam(fixCam, evt.blend);
                break;

            case CameraType.EyeEffectCam:
                SelectCam(eyeeffect, evt.blend);
                break;

            //case CameraType.EnemyCam:
            //    SelectCam(enemyCam, evt.blend);
            //    break;

            //case CameraType.BetweenCam:
            //    SelectCam(betweenCam, evt.blend);
            //    break;
        }
    }
}
