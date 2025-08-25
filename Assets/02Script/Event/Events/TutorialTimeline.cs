using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
///  튜토리얼 내 타임라인 Detail
/// </summary>

public class TutorialTimeline : MonoBehaviour
{
    public string eventID = "E002";
    [SerializeField] private GameObject dialog;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI textarea;
    [SerializeField] private TextMeshProUGUI speaker;

    private IDatabase database;
    private List<DialogData> dialogs;  // 전체 대사 목록
    int curidx = 0;  // 현재 대화 인덱스


    // DB 연결
    public void Init(IDatabase db) {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialTimeline - Failed to Load DialogData");
    }

    // signal때 재생될 함수
    public void OnSignal() {
        if (curidx < dialogs.Count) {
            ShowDialog(dialogs[curidx]);
            StartCoroutine(OnWaiting());
            curidx++;
        }
    }

    void ShowDialog(DialogData log) {
        group.alpha = 1f;
        speaker.text = log.speakerName;
        textarea.text = log.dialog;
    }

    IEnumerator OnWaiting() {
        yield return new WaitForSeconds(2.4f);

        // 대화창 끄기
        group.alpha = 0f;
        textarea.text = "";
        speaker.text = "";
    }
}
