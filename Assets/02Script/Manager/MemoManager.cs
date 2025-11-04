using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>

public class MemoManager : MonoBehaviour
{
    [Header("Memo UI")]
    [SerializeField] private Image[] pieces;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Button memoBTN;
    //[SerializeField] private Button memoTab;

    // 허용하는 itemID 목록
    private readonly int[] allowedIDs = { 50001001, 50001002, 50001003, 50001004 };

    private void Awake()
    {
        // 초기값 : 모두 꺼둠
        foreach (var piece in pieces) { 
            piece.enabled = false;
        }

        foreach (var text in texts) { 
            text.enabled = false;
        }
        UpdateMemoButton(); // 버튼 초기상태
    }

    private void OnEnable()
    {
        UpdateMemoText();
        UpdateMemoImg();
        UpdateMemoButton();

        EventBus.Instance.Subscribe<GameEvents.GetNote>(OnGetNote);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GetNote>(OnGetNote);
    }

    private void OnGetNote(GameEvents.GetNote evt) {
        // itemID가 허용된 목록에 없으면 무시
        if (System.Array.IndexOf(allowedIDs, evt.itemID) < 0)
            return;

        // 이미존재하면 무시
        if (EventHistoryManager.Instance.noteCollection.ContainsKey(evt.itemID)) {
            Debug.Log($"이미 획득한 메모 : {evt.text}");
            return;
        }

        // 추가
        EventHistoryManager.Instance.noteCollection[evt.itemID] = evt.text;
        Debug.Log("노트추가완료!");

        UpdateMemoText();
        UpdateMemoImg();
        UpdateMemoButton();
    }

    private void UpdateMemoImg() {
        // id배열 순서대로 각 아이템 활성화
        if (pieces != null) { 
            for (int i = 0; i < allowedIDs.Length && i < pieces.Length; i++)
            {
                pieces[i].enabled = EventHistoryManager.Instance.noteCollection.ContainsKey(allowedIDs[i]);
            }
        }

    }

    private void UpdateMemoText() {
        // cnt 수만큼 배열 앞부터 활성화 / cnt가 배열 길이보다 크면 배열 길이까지만 활성화
        if (texts != null) { 
            for (int i = 0; i < allowedIDs.Length && i < texts.Length; i++)
            {
                // 해당 allowedID가 noteCollection에 있다면 활성화 및 텍스트 지정
                if (EventHistoryManager.Instance.noteCollection.ContainsKey(allowedIDs[i]))
                {
                    texts[i].enabled = true;
                    texts[i].text = EventHistoryManager.Instance.noteCollection[allowedIDs[i]];
                }
                else
                {
                    texts[i].enabled = false;
                    texts[i].text = ""; // 비워두기
                }
            }
        }
    }

    private void UpdateMemoButton()
    {
        //if (memoBTN != null && memoTab != null)
        if (memoBTN != null)
            {
            // 메모가 하나도 없으면 버튼 비활성화
            memoBTN.interactable = EventHistoryManager.Instance.noteCollection.Count > 0;
            //memoTab.interactable = EventHistoryManager.Instance.noteCollection.Count > 0;
        }
    }
}
