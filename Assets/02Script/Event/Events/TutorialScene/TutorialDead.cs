using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 튜토리얼에서 DeadScene 이후 대사 + 씬이동
/// </summary>

public class TutorialDead : MonoBehaviour
{
    public string eventID = "E004";
    private IDatabase database;
    private PlayerController pc;
    private Dictionary<int, DialogData> dialogs;

    // DB 연결
    public void Init(IDatabase db)
    {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialDead - Failed to Load DialogData");
    }

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("TutorialDead - Failed to Load PlayerController");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.PlayEvent>(OnEvent);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.PlayEvent>(OnEvent);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }


    private void OnEvent(GameEvents.PlayEvent evt) {
        if (evt.eventID == this.eventID) {
            // 대화 모드
            //pc.CurMode = GameMode.DialogMode;
            //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));

            EventBus.Instance.Publish<UIEvents.OpenMonologue>(new UIEvents.OpenMonologue(eventID, dialogs));
        }
    }

    private void OnEndDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == this.eventID) {

            // HappyScene으로 이동
            EventBus.Instance.Publish<GameEvents.SwitchScene>(new GameEvents.SwitchScene(SceneType.HappyScene));
        }
    }
}
