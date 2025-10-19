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

    private PlayerController pc;
    private PlayerMove pm;
    private EventHistoryManager hm;
    private IDatabase database;
    private Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    private Dictionary<int, DialogData> initialDialog;

    public string ZoneID => zoneID;


    public void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("BlockTriggerZone - Failed to Load Dialog");
    }


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerController");
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerMove");
        hm = FindAnyObjectByType<EventHistoryManager>();
        if (hm == null) Debug.Log("BlockTriggerZone - Failed to Load EventHistoryManager");
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
    }



    private void OnTriggerEnter(Collider other)
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

    public void OnTrigger(GameObject actor)
    {
        if (canPass)
        {
            // zone 상태 저장
            EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
        }
        else 
        {
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));

            // 종료 이벤트 저장
            EventBus.Instance.Publish<GameEvents.EndEvent>(new GameEvents.EndEvent(eventID));
        }
    }


    private void OnEndDialog(UIEvents.EndDialog evt) {
        // 경고 대화가 끝났을 때
        if (evt.eventID == eventID) {
            pm.PushBack();
        }
    }

}
