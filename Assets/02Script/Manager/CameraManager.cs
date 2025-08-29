using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera fixCam;
    [SerializeField] private CinemachineVirtualCamera sightCam;
    [SerializeField] private CinemachineVirtualCamera enemyCam;
    [SerializeField] private CinemachineVirtualCamera betweenCam;
    [SerializeField] private Button toggleBTN;

    private List<CinemachineVirtualCamera> cams;
    private bool isPlayer = false;
    private Camera mainCam;
    private CinemachineBrain brain;

    private void Awake()
    {
        mainCam = Camera.main;
        brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null) Debug.Log("CameraManager - Failed to Load CinemachineBrain");
        cams = new List<CinemachineVirtualCamera> { fixCam, sightCam, enemyCam, betweenCam };
    }

    private void Start()
    {
        sightCam.Priority = 0;
        fixCam.Priority = 10;
    }

    private void OnEnable()
    {
        toggleBTN.onClick.AddListener(Onoff);
        EventBus.Instance.Subscribe<GameEvents.CameraShift>(OnCameraShift);
    }
    private void OnDisable()
    {
        toggleBTN.onClick.RemoveListener(Onoff);
        EventBus.Instance.Unsubscribe<GameEvents.CameraShift>(OnCameraShift);
    }

    private void Onoff() {
        if (isPlayer)
        {
            sightCam.Priority = 0;
            fixCam.Priority = 10;
            mainCam.orthographic = true;
        }
        else {
            fixCam.Priority = 0;
            sightCam.Priority = 10;
            mainCam.orthographic = false;
        }
        isPlayer = !isPlayer;
    }

    private void OnCameraShift(GameEvents.CameraShift evt) {
        switch (evt.type)
        {
            case CameraType.PlayerFixCam:
                SelectCam(fixCam, evt.blend);
                break;

            case CameraType.PlayerSightCam:
                SelectCam(sightCam, evt.blend);
                break;

            case CameraType.EnemyCam:
                SelectCam(enemyCam, evt.blend);
                break;

            case CameraType.BetweenCam:
                SelectCam(betweenCam, evt.blend);
                break;
        }
    }

    private void SelectCam(CinemachineVirtualCamera onCam, float blend)
    {
        //blend 조절
        brain.m_DefaultBlend.m_Time = blend;

        // 모두 끈 후
        foreach (var cam in cams)
        {
            cam.Priority = (cam == onCam) ? 11 : 0;
        }
    }
}
