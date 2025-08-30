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
    private Dictionary<int, DialogData> dialogs;

    // DB 연결
    public void Init(IDatabase db)
    {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialDead - Failed to Load DialogData");

        Debug.Log($"dialogs 잘받아오나?????? : {dialogs.Count}");
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
            // 대화 실행 (1)
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));
            Debug.Log("대화실행 in DeadScene");
        }
    }

    private void OnEndDialog(UIEvents.EndDialog evt) {
        if (evt.eventID == this.eventID) {
            // HappyScene으로 이동
            SceneManager.LoadScene("HappyScene");
            Debug.Log("해피씬으로 이동~!");
        }
    }
}
