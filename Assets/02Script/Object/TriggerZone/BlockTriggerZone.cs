using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public bool canPass = false;
    public bool isContacted = false;

    //[Header("Barrier")]
    //[SerializeField] private BoxCollider barrier;

    private PlayerController pc;
    private PlayerMove pm;
    private EventHistoryManager hm;
    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;
    private BoxCollider barrier;

    // 임시
    public string needEventID;


    public void Init(IDatabase db)
    {
        database = db;
        dialogs = database.GetDialog(eventID);
    }


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerController");
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("BlockTriggerZone - Failed to Load PlayerMove");
        hm = FindAnyObjectByType<EventHistoryManager>();
        if (hm == null) Debug.Log("BlockTriggerZone - Failed to Load EventHistoryManager");

        barrier = GetComponentInChildren<BoxCollider>();
        if (barrier == null) Debug.Log("BlockTriggerZone - Failed to Load BarrierCollider");
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
            if (hm.IsEventComplete(needEventID)) { 
                canPass = true;
            }

            OnTrigger(other.gameObject);
        }
    }

    public void OnTrigger(GameObject actor)
    {
        if (canPass)
        {
            Debug.Log("통과 가능");
            barrier.gameObject.SetActive(false);        // 방해물 제거
        }
        else 
        {
            Debug.Log("통과 불가능");
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));

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
