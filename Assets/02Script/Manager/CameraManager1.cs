using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

//public class CameraManager : MonoBehaviour
//{
//    [SerializeField] private CinemachineVirtualCamera playerCam;
//    [SerializeField] private List<CinemachineVirtualCamera> cams;

//    private void OnEnable()
//    {
//        EventBus.Instance.Subscribe<GameEvents.EndTimeline>(OnEndTimeline);
//    }
//    private void OnDisable()
//    {
//        EventBus.Instance.Unsubscribe<GameEvents.EndTimeline>(OnEndTimeline);
//    }

//    private void OnEndTimeline(GameEvents.EndTimeline evt) {
//        RefreshCam();
//    }

//    // 플레이어 카메라로 돌아오기
//    private void RefreshCam() {
//        foreach (var cam in cams) { 
//            cam.gameObject.SetActive(false);
//        }
//        playerCam.gameObject.SetActive(true);
//    }
//}
