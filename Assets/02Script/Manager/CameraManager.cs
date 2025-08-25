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

    private void Awake()
    {
        mainCam = Camera.main;
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
                SelectCam(fixCam);
                break;

            case CameraType.PlayerSightCam:
                SelectCam(sightCam);
                break;

            case CameraType.EnemyCam:
                SelectCam(enemyCam);
                break;

            case CameraType.BetweenCam:
                SelectCam(betweenCam);
                break;
        }
    }

    private void SelectCam(CinemachineVirtualCamera onCam)
    {
        // ¸ðµÎ ²ö ÈÄ
        foreach (var cam in cams)
        {
            cam.Priority = (cam == onCam) ? 11 : 0;
        }
    }
}
