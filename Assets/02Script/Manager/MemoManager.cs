using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 지금은 개수대로 나오게했는데 추후에 누락된부분만 따로 떨어지게 다시 코딩할것~~
/// </summary>

public class MemoManager : MonoBehaviour
{
    [Header("Memo UI")]
    [SerializeField] private Image[] pieces;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Button memoBTN;

    public static Dictionary<int, string> noteCollection = new();

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
        UpdateMemoText(noteCollection.Count);
        UpdateMemoImg(noteCollection.Count);
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
        if (noteCollection.ContainsKey(evt.itemID)) {
            Debug.Log($"이미 획득한 메모 : {evt.text}");
            return;
        }
        
        // 추가
        noteCollection[evt.itemID] = evt.text;
        Debug.Log("노트추가완료!");

        UpdateMemoText(noteCollection.Count);
        UpdateMemoImg(noteCollection.Count);
        UpdateMemoButton();
    }

    private void UpdateMemoImg(int cnt) {
        // cnt 수만큼 배열 앞부터 활성화 / cnt가 배열 길이보다 크면 배열 길이까지만 활성화
        if (pieces != null) { 
            for (int i = 0; i < cnt && i < pieces.Length; i++)
            {
                pieces[i].enabled = true;
            }
        }

    }

    private void UpdateMemoText(int cnt) {
        // cnt 수만큼 배열 앞부터 활성화 / cnt가 배열 길이보다 크면 배열 길이까지만 활성화
        if (texts != null) { 
            for (int i = 0; i < cnt && i < texts.Length; i++)
            {
                texts[i].enabled = true;
            }
        }
    }

    private void UpdateMemoButton()
    {
        if (memoBTN != null)
        {
            // 메모가 하나도 없으면 버튼 비활성화
            memoBTN.interactable = noteCollection.Count > 0;
        }
    }
}
