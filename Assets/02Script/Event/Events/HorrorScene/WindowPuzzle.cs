using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 창문마다 고유의 windowID(int) 가짐

// 1층 : 60001001 , 60001002, 60001003, 60001004
// 2층 : 60002001 , 60002002, 60002003, 60002004

public enum HorrorFloor
{ 
    none,
    Floor1,
    Floor2,
    Floor3,
}

public class WindowPuzzle : MonoBehaviour
{
    [Header("Visualization")]
    [SerializeField] private bool is1FComplete = false;
    [SerializeField] private bool is2FComplete = false;

    // 이벤트에 대한 필요조건
    private readonly HashSet<int> req1F = new HashSet<int> { 60001001, 60001002, 60001003, 60001004 };
    private readonly HashSet<int> req2F = new HashSet<int> { 60002001, 60002002, 60002003, 60002004 };

    // 현재 진행상황
    private HashSet<int> cur1F = new HashSet<int>();
    private HashSet<int> cur2F = new HashSet<int>();


    // 이벤트 관련 대화
    private IDatabase database;
    private Dictionary<string, Dictionary<int, DialogData>> all1FDialogs;
    private Dictionary<string, Dictionary<int, DialogData>> all2FDialogs;
    private Dictionary<int, DialogData> initial1FDialog;
    private Dictionary<int, DialogData> initial2FDialog;

    private string event1FID = "E068";
    private string event2FID = "E074";

    public void Init(IDatabase db)
    {
        database = db;
        all1FDialogs = database.GetDialogEvent(event1FID);
        all2FDialogs = database.GetDialogEvent(event2FID);
        initial1FDialog = database.GetDialog(event1FID, event1FID);
        initial2FDialog = database.GetDialog(event2FID, event2FID);
        if (initial1FDialog == null) Debug.Log("WindowPuzzle - Failed to Load 1FDialog");
        if (initial2FDialog == null) Debug.Log("WindowPuzzle - Failed to Load 2FDialog");
    }


    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PuzzleEvents.H_CheckWindow>(OnCheckWindow);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PuzzleEvents.H_CheckWindow>(OnCheckWindow);
    }

    private void OnCheckWindow(PuzzleEvents.H_CheckWindow evt) {
        // 이벤트를 기록

        // 1층이면,
        if (evt.floor == HorrorFloor.Floor1)
        {
            cur1F.Add(evt.windowID);
        }
        // 2층이면
        else if (evt.floor == HorrorFloor.Floor2)
        {
            cur2F.Add(evt.windowID);
        }

        // 창문 이벤트 벌어질때마다 전체확인
        CheckWholeEvent(evt.floor);
    }

    private void CheckWholeEvent(HorrorFloor floor) {
        if (floor == HorrorFloor.Floor1 && !is1FComplete)
        {
            if (cur1F.SetEquals(req1F))
            {
                Debug.Log("*********1층 창문 모두확인");
                is1FComplete = true;
                EventBus.Instance.Publish(new PuzzleEvents.H_GetPassword(floor));

                // 대화이벤트 발생
                //PublishEvent(floor);
            }
        }
        else if(floor == HorrorFloor.Floor2 && !is2FComplete)
        {
            if (cur2F.SetEquals(req2F))
            {
                Debug.Log("*********2층 창문 모두확인");
                is2FComplete = true;
                EventBus.Instance.Publish(new PuzzleEvents.H_GetPassword(floor));

                // 대화이벤트 발생
                //PublishEvent(floor);
            }
        }
    }

    // 4개를 전부 확인하면 강제 이벤트 발생
    private void PublishEvent(HorrorFloor floor) 
    {
        if (floor == HorrorFloor.Floor1) 
        {
            if (all1FDialogs != null) {
                PlayerController.Instance.CurMode = GameMode.EventMode;
                EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));

                EventBus.Instance.Publish(new UIEvents.OpenDialog(event1FID, initial1FDialog, GameMode.InspectMode));
            }
        }
        else if (floor == HorrorFloor.Floor2)
        {
            if (all2FDialogs != null)
            {
                PlayerController.Instance.CurMode = GameMode.EventMode;
                EventBus.Instance.Publish(new GameEvents.GameModeChange(GameMode.EventMode));

                EventBus.Instance.Publish(new UIEvents.OpenDialog(event2FID, initial2FDialog, GameMode.InspectMode));
            }
        }
    }
}
