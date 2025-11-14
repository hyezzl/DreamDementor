using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public bool isContacted = false;

    protected PlayerController pc;
    protected IDatabase database;
    protected Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    protected Dictionary<int, DialogData> initialDialog;

    public string ZoneID => zoneID;

    public void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("TriggerZone - Failed to Load Dialog");
    }

    protected virtual void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("TriggerZone - Failed to Load PlayerController");
    }

    protected void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
    }
    protected void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
    }

    public virtual void OnTrigger(GameObject actor)
    {
        if (!isContacted && initialDialog != null)
        {
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
        }
    }

    public virtual void OnEndDialog(UIEvents.EndDialog evt) {
        // 게임모드 변경
        if (evt.eventID == eventID) { 
            Debug.Log($"{evt.eventID} 이벤트 트리거 삭제@");
            DestroyTrigger();
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            OnTrigger(other.gameObject);
        }
    }

    protected void DestroyTrigger() {
        // 한번 발생 후 파괴
        // 파괴 ? 하는게 좋나
        //Destroy(gameObject);
        EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
        gameObject.SetActive(false);
    }
}
