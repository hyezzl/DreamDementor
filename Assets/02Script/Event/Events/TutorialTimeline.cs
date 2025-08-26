using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

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
    private Dictionary<int, DialogData> dialogs;  // 전체 대사 목록
    int curlogIdx;  // 현재 대화 인덱스


    // DB 연결
    public void Init(IDatabase db) {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialTimeline - Failed to Load DialogData");

        curlogIdx = dialogs.Keys.Min();  // 해당 이벤트 내 가장 처음 로그인덱스 저장
    }

    // signal때 재생될 함수
    public void OnSignal() {

        if (!dialogs.ContainsKey(curlogIdx))
        {
            group.alpha = 0f;  // 대사 끝나면 UI 숨김
            return;
        }
        StartCoroutine(PlayDialogCoroutine(curlogIdx));
        curlogIdx = dialogs[curlogIdx].nextID;

    }

    private IEnumerator PlayDialogCoroutine(int logID)
    {
        var dialog = dialogs[logID];

        group.alpha = 1f;
        speaker.text = dialog.speakerName;
        yield return textarea.DOText(dialog.dialog, 1.7f);

        yield return new WaitForSeconds(2.4f);

        textarea.text = "";
        speaker.text = "";
    }

    void ShowDialog(DialogData log) {
        speaker.text = log.speakerName;
        // 타이핑
        textarea.DOText(log.dialog, 1.7f);
    }

    //IEnumerator OnWaiting() {
    //    yield return new WaitForSeconds(2.4f);

    //    // 대화창 초기화
    //    textarea.text = "";
    //    speaker.text = "";
    //}
}
