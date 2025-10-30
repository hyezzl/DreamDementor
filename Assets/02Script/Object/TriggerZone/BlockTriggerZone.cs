using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public bool canPass = false;
    public bool isContacted = false;
    public string deactiveConditionID;  // 블락 비활성화 조건

    protected PlayerController pc;
    protected PlayerMove pm;
    protected EventHistoryManager hm;
    protected IDatabase database;
    protected Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    protected Dictionary<int, DialogData> initialDialog;

    public string ZoneID => zoneID;


    public virtual void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("BlockTriggerZone - Failed to Load Dialog");
    }


    protected virtual void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerController");
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerMove");
        hm = FindAnyObjectByType<EventHistoryManager>();
        if (hm == null) Debug.Log("BlockTriggerZone - Failed to Load EventHistoryManager");
    }

    protected void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    protected void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
    }



    protected virtual void OnTriggerEnter(Collider other)
    {
        // 트리거 존은 플레이어와만 상호작용
        if (other.CompareTag("Player")) {
            // 조건 이벤트 확인
            if (hm.IsEventComplete(deactiveConditionID)) { 
                canPass = true;
            }

            OnTrigger(other.gameObject);
        }
    }

    public virtual void OnTrigger(GameObject actor)
    {
        if (canPass)
        {
            // zone 상태 저장
            EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
        }
        else 
        {
            EventBus.Instance.Publish<GameEvents.PlayEvent>(new GameEvents.PlayEvent(eventID));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));

            // 종료 이벤트 저장
            EventBus.Instance.Publish<GameEvents.EndEvent>(new GameEvents.EndEvent(eventID)); // 이거는 대화에서 하는거아닌가?
        }
    }


    protected virtual void OnEndDialog(UIEvents.EndDialog evt) {
        // 경고 대화가 끝났을 때
        if (evt.eventID == eventID) {
            pm.PushBack();
        }
    }

}
