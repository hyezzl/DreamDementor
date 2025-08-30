using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 튜토리얼 종료 후 해피씬으로 강제이동 후 바로 실행되는 이벤트
/// </summary>
public class TeleportHappyScene : MonoBehaviour
{
    [SerializeField] private string eventID = "E005";

    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;
    private PlayerController pc;

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

        PlayTeleport();
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
    }

    private void PlayTeleport() {

        // 눈열리는 연출

        // 게임모드 변경
        //(임시)
        //pc.CurMode = GameMode.InspectMode;
        //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));

        // 이벤트 모드로 변경

        // 이벤트 시작 (대화)
        EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));

        // 대화모드
        pc.CurMode = GameMode.DialogMode;
        EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

        EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));

    }

    private void OnEndDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == this.eventID) {
            // 대화가 끝났을 때 게임모드 변경
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }
    }
}
