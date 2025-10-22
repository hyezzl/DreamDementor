using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
/// 튜토리얼 종료 후 해피씬으로 강제이동 후 바로 실행되는 이벤트
/// </summary>
public class TeleportHappyScene : EventBase
{
    [SerializeField] PlayableDirector eyesOpen;
    [SerializeField] private Canvas eyeCanvas;

    private static bool HappySceneFirstVisit = true;

    public override void Init(IDatabase db)  // start시점에 실행
    {
        base.Init(db);

        // PlayerPrefs사용한 첫방문 분기
        //bool happyFirstVisit = PlayerPrefs.GetInt("HappySceneFirstVisit", 0) == 0;  // 첫방문인가?

        //if (happyFirstVisit) { 
        //    eyeCanvas.gameObject.SetActive(true);
        //    PlayTeleport();
        //    PlayerPrefs.SetInt("HappySceneFirstVisit", 1);
        //    PlayerPrefs.Save();

        //    //브금 재생
        //}

        if (HappySceneFirstVisit) {
            eyeCanvas.gameObject.SetActive(true);
            PlayTeleport();
            Debug.Log("HappyScene 임시로 재방문켜둠!");

            // 브금 재생
            EventBus.Instance.Publish<GameEvents.PlayBGM>(new GameEvents.PlayBGM(BGMType.HappyBGM));
        }
    }

    private void PlayTeleport() {
        StartCoroutine(PlayEvents());
    }

    private IEnumerator PlayEvents() {
        // 눈열리는 연출
        eyesOpen.Play();
        yield return new WaitUntil(() => eyesOpen.state != PlayState.Playing);

        // 이벤트 시작 (대화)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));

        // 대화모드
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));
    }

    protected override void CloseDialog(UIEvents.EndDialog evt) {
        base.CloseDialog(evt);
        
        if (evt.eventID == this.eventID) {
            // 카메라 고정
            EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.PlayerFixCam, 1));
        }
    }
}
