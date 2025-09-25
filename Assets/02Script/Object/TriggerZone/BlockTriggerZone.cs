using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public bool canPass = false;
    public bool isContacted = false;
    [SerializeField] private Transform player;

    private PlayerController pc;
    private PlayerMove pm;
    private IDatabase database;
    private Dictionary<int, DialogData> dialogs;

    // 임시
    public string needEventID = "E007";


    public void Init(IDatabase db)
    {
        database = db;
        dialogs = database.GetDialog(eventID);
    }


    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("TriggerZone - Failed to Load PlayerController");
        pm = FindAnyObjectByType<PlayerMove>();
        if (pm == null) Debug.Log("TriggerZone - Failed to Load PlayerMove");
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
            if (pc.IsEventComplete(needEventID)) { 
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
        }
        else 
        {
            Debug.Log("통과 불가능");

            // 강제 대화이벤트 발생
            //pc.CurMode = GameMode.DialogMode;
            //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.DialogMode));
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, dialogs));

            // 종료 이벤트 저장


            // 플레이어 뒤로 밈
            // 대화가 끝났으면
        }
    }


    private void OnEndDialog(UIEvents.EndDialog evt) {
        // 경고 대화가 끝났을 때
        if (evt.eventID == eventID) {
            pm.PushBack();
        }
    }

}
