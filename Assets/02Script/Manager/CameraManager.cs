using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera lookat;
    [SerializeField] private CinemachineVirtualCamera playersight;
    [SerializeField] private Button toggleBTN;

    private bool isPlayer = false;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        playersight.Priority = 0;
        lookat.Priority = 10;
    }

    private void OnEnable()
    {
        toggleBTN.onClick.AddListener(Onoff);
    }
    private void OnDisable()
    {
        toggleBTN.onClick.RemoveListener(Onoff);
    }

    private void Onoff() {
        if (isPlayer)
        {
            playersight.Priority = 0;
            lookat.Priority = 10;
            mainCam.orthographic = true;
        }
        else {
            lookat.Priority = 0;
            playersight.Priority = 10;
            mainCam.orthographic = false;
        }
        isPlayer = !isPlayer;
    }
}
