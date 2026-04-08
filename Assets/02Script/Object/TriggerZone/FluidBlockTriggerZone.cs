using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 블락 활성시점에 조건분기
/// </summary>

public class FluidBlockTriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public string activeConditionID;    // 블락 활성화 조건
    public string deactiveConditionID;  // 블락 비활성화 조건
    public bool canPass = true;

    private EventHistoryManager hm;
    private PlayerMove pm;
    private Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    private Dictionary<int, DialogData> initialDialog;
    private IDatabase database;


    public string ZoneID => zoneID;

    public void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("FluidBlockTriggerZone - Failed to Load Dialog");
    }

    private void Awake()
    {
        hm = FindAnyObjectByType<EventHistoryManager>();
        pm = FindAnyObjectByType<PlayerMove>();
        canPass = true;
    }
    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndDialog>(OnEndDialog);
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndDialog>(OnEndDialog);
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!barrier.gameObject.activeSelf) return; // block 비활성 시 무시

        if (other.CompareTag("Player"))
        {
            OnTrigger(other.gameObject);
        }
    }

    public void OnTrigger(GameObject actor)
    {
        if (hm != null && !hm.IsEventComplete(activeConditionID))
        {
            // 아직 벽이 생성될 조건이 안 되었다면, 밟아도 아무 기록도 하지 않고 통과시킴
            return;
        }

        if (canPass)
        {
            if (hm != null && hm.IsEventComplete(deactiveConditionID))
            {
                EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
                gameObject.SetActive(false);
            }
        }
        else
        {
            // 벽이 생긴 상태
            if (initialDialog == null)
            {
                Debug.LogError($"initialDialog가 null ZoneID: {zoneID}, EventID: {eventID}");
            }
            else
            {
                EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog, GameMode.InspectMode));
            }
        }
    }


    // 활성화 / 비활성화 이벤트 관리

    private void OnEndDialog(UIEvents.EndDialog evt)
    {
        // 활성화 이벤트가 끝났을 경우 block 활성화
        if (evt.eventID == this.activeConditionID) {
            canPass = false;
        }

        // 블락이벤트 대화가 끝나면 플레이어 뒤로 강제 후진
        if (evt.eventID == eventID)
        {
            pm.PushBack();
        }

        // 비활성화 이벤트가 끝났을 경우 block 비활성화
        if (evt.eventID == this.deactiveConditionID)
        {
            canPass = true;
        }
    }

    
    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        // 활성화 이벤트가 끝났을 경우 block 활성화
        if (evt.npcID == this.activeConditionID)
        {
            canPass = false;
        }

        // 비활성화 이벤트가 끝났을 경우 block 비활성화
        if (evt.npcID == this.deactiveConditionID)
        {
            canPass = true;
        }
    }
}
