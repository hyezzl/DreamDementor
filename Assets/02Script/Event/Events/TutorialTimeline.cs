using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;
using Cinemachine;

/// <summary>
///  튜토리얼 내 타임라인 Detail
/// </summary>

public class TutorialTimeline : MonoBehaviour
{
    public string eventID = "E002";
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup playerTextBox;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI playerSpeaker;

    [SerializeField] private CanvasGroup flipTextBox;
    [SerializeField] private TextMeshProUGUI flipText;
    [SerializeField] private TextMeshProUGUI flipSpeaker;


    [Header("Objects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private CinemachineVirtualCamera betweencam;

    [Header("타이핑 속도")]
    [SerializeField] private float TypingTime = 1.7f;

    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;  // 전체 대사 목록
    int curlogIdx;  // 현재 대화 인덱스
    Vector3 timelineLastPosPlayer; // 플레이어 마지막 위치
    Vector3 timelineLastPosEnemy;
    Vector3 betweenCamLastPos;


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
            // 대사 끝나면 UI 숨김
            playerTextBox.alpha = 0f;  
            flipTextBox.alpha = 0f;
            return;
        }
        StartCoroutine(PlayDialogCoroutine(curlogIdx));
        curlogIdx = dialogs[curlogIdx].nextID;

    }

    private IEnumerator PlayDialogCoroutine(int logID)
    {
        var dialog = dialogs[logID];

        if (dialog.speaker == Speaker.Player)
        {
            SetCanvasGroup(playerTextBox, true);
            SetCanvasGroup(flipTextBox, false);
            playerSpeaker.text = dialog.speakerName;
            yield return playerText.DOText(dialog.dialog, TypingTime);
        }
        else {
            SetCanvasGroup(playerTextBox, false);
            SetCanvasGroup(flipTextBox, true);
            flipSpeaker.text = dialog.speakerName;
            yield return flipText.DOText(dialog.dialog, TypingTime);
        }

        yield return new WaitForSeconds(2.4f);

        playerText.text = "";
        flipText.text = "";
        playerSpeaker.text = "";
        flipSpeaker.text = "";
    }

    private void SetCanvasGroup(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1f : 0f;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }

    public void UpdateLastPositions()
    {
        // 캐릭터 오브젝트들의 마지막 Position 저장
        timelineLastPosPlayer = player.transform.position;
        timelineLastPosEnemy = enemy.transform.position;
        betweenCamLastPos = betweencam.transform.position;
    }
    public void SaveLastPosition() {
        // 적용
        player.transform.position = timelineLastPosPlayer;
        enemy.transform.position = timelineLastPosEnemy;
        betweencam.transform.position = betweenCamLastPos;
    }
}
