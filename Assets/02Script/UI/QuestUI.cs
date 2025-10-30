using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using static GameEvents;

public class QuestUI : MonoBehaviour, IGameEvent
{

    [Header("UI Ref")]
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private TextMeshProUGUI text;

    // DB
    private IDatabase database;
    private string curQuest;
    private List<string> allOpenQuest = new();
    private List<string> allCloseQuest = new();

    private string preText = "●  ";

    private void Awake()
    {
        canvas.alpha = 0f;
    }

    public virtual void Init(IDatabase db)
    {
        database = db;
        Debug.Log($"*{this.GetType().Name} 의 Init 완료!");

        allOpenQuest = database.GetOpenQuestList();
        allCloseQuest = database.GetCloseQuestList();
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Subscribe<GameEvents.EndEvent>(OnEndEvent);
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);

    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<GameEvents.GameModeChange>(OnModeChange);
        EventBus.Instance.Unsubscribe<GameEvents.EndEvent>(OnEndEvent);
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);

    }

    private void OnEndEvent(GameEvents.EndEvent evt) { 
        
    }


    private void OnEndDialog(UIEvents.EndDialog evt) {

        // 오픈퀘스트 목록에 있을 때 (Close보다 우선)
        if (allOpenQuest.Contains(evt.eventID))
        {
            // database에서 내용가져오기
            curQuest = database.GetOpenQuest(evt.eventID);
            if (!string.IsNullOrEmpty(curQuest))
            {
                text.text = preText + curQuest;

                Debug.Log($"퀘스트 창 열기: {curQuest}");
                OpenQuest();
            }
        }
        // 클로즈 퀘스트 목록에 있을 때
        else if (allCloseQuest.Contains(evt.eventID)) {
            Debug.Log($"퀘스트 창 닫기: {evt.eventID}");
            CloseQuest();
        }
    }

    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        // 오픈퀘스트 목록에 있을 때 (Close보다 우선)
        if (allOpenQuest.Contains(evt.npcID))
        {
            // database에서 내용가져오기
            curQuest = database.GetOpenQuest(evt.npcID);
            if (!string.IsNullOrEmpty(curQuest))
            {
                Debug.Log($"퀘스트 창 열기: {curQuest}");
                text.text = preText + curQuest;

                OpenQuest();
            }
        }
        // 클로즈 퀘스트 목록에 있을 때
        else if (allCloseQuest.Contains(evt.npcID))
        {
            Debug.Log($"퀘스트 창 닫기: {evt.npcID}");
            CloseQuest();
        }
    }




    // 모드 변경 시 숨김
    private void OnModeChange(GameEvents.GameModeChange evt) {
        if (evt.mode == GameMode.InspectMode || evt.mode == GameMode.PauseMode)
        {
            OpenQuest();
        }
        else 
        {
            // Inspect모드 제외 + 대화 + 이벤트 + 게임오버
            CloseQuest();
        }
    }

    private void OpenQuest() { canvas.alpha = 1f; }
    private void CloseQuest() { 
        canvas.alpha = 0f;
        curQuest = null;
    }


}
