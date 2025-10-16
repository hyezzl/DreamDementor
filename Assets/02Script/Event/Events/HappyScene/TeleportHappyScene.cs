using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
/// 튜토리얼 종료 후 해피씬으로 강제이동 후 바로 실행되는 이벤트
/// </summary>
public class TeleportHappyScene : MonoBehaviour
{
    [SerializeField] PlayableDirector eyesOpen;

    [SerializeField] private string eventID = "E005";

    [SerializeField] private Canvas eyeCanvas;

    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;
    private PlayerController pc;

    private static bool HappySceneFirstVisit = true;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("TeleportHappyScene - Failed to Load PlayerController");
    }

    public void Init(IDatabase db)  // start시점에 실행
    {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TeleportHappyScene - Failed to Load NarrationData");

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

            // 브금 재생
            EventBus.Instance.Publish<GameEvents.PlayBGM>(new GameEvents.PlayBGM(BGMType.HappyBGM));
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
    }

    private void Update()
    {
        // 임시
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            OnEndDialog(new UIEvents.EndDialog("E005"));
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
        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));
    }

    private void OnEndDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == this.eventID) {
            // 대화가 끝났을 때 게임모드 변경
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

            // 카메라 고정
            EventBus.Instance.Publish<GameEvents.CameraShift>(new GameEvents.CameraShift(CameraType.PlayerFixCam, 1));
        }
    }
}
