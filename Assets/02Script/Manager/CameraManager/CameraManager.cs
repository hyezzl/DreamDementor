using Cinemachine;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cams;
    protected bool isPlayer = false;
    protected Camera mainCam;
    protected CinemachineBrain brain;

    protected virtual void Awake()
    {
        mainCam = Camera.main;
        brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null) Debug.Log("CameraManager - Failed to Load CinemachineBrain");
        if(cams == null) cams = new List<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.CameraShift>(OnCameraShift);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.CameraShift>(OnCameraShift);
    }


    protected virtual void OnCameraShift(GameEvents.CameraShift evt) {
    }

    

    protected void SelectCam(CinemachineVirtualCamera onCam, float blend = 1)
    {
        //blend Á¶Àý
        brain.m_DefaultBlend.m_Time = blend;

        if (cams != null) { 
            foreach (var cam in cams)
            {
                cam.Priority = (cam == onCam) ? 11 : 0;
            }
        }
    }
}
