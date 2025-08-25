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
    private List<DialogData> dialogs;


    // DB 연결
    public void Init(IDatabase db) {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("TutorialTimeline - Failed to Load DialogData");
    }

    // signal때 재생될 함수
    public void OnSignal(List<DialogData> texts) {
        // 대화창 설정 (초기화) 끝에 할까?
        //group.alpha = 1f; // 필요하면 Lerp
        //textarea.text = "";
        //speaker.text = "";

        // 데이터 넣기 //////////////
        StartCoroutine(OnWaiting());
    }

    IEnumerator OnWaiting() {

        // 대화창 설정 (초기화) 끝에 할까?
        group.alpha = 1f; // 필요하면 Lerp
        textarea.text = "";
        speaker.text = "";
        yield return null;

        // 대화창 (for ++)

        yield return new WaitForSeconds(2.5f);

        // 대화창 끄기
        group.alpha = 0f;
        textarea.text = "";
        speaker.text = "";
    }
}
