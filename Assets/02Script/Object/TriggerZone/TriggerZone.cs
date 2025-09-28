using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public bool isContacted = false;

    private PlayerController pc;
    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;

    public void Init(IDatabase db)
    {
        database = db;
        dialogs = database.GetDialog(eventID);
        if (dialogs == null) Debug.Log("");
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
        if (!isContacted && dialogs != null)
        {
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));
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
        gameObject.SetActive(false);
    }
}
