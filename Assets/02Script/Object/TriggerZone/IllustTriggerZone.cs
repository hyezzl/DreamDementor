using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllustTriggerZone : MonoBehaviour, ITriggerZone
{
    public string zoneID;
    public string eventID;
    public string illID;
    public bool isContacted = false;

    private PlayerController pc;
    private GameMode preMode;
    private IDatabase database;
    private Dictionary<string, Dictionary<int, DialogData>> allDialogs;
    private Dictionary<int, DialogData> initialDialog;

    public string ZoneID => zoneID;


    public void Init(IDatabase db)
    {
        database = db;
        allDialogs = database.GetDialogEvent(eventID);
        initialDialog = database.GetDialog(eventID, eventID);
        if (initialDialog == null) Debug.Log("IllustTriggerZone - Failed to Load Dialog");
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
        if (!isContacted) {
            // 일러스트 n초간 전시
            EventBus.Instance.Publish<UIEvents.SceneCover>(new UIEvents.SceneCover(illID));
            StartCoroutine(DelayAfter(2.5f));

            // 게임모드
            preMode = pc.CurMode;
            pc.CurMode = GameMode.EventMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.EventMode));
        }
    }

    public virtual void OnEndDialog(UIEvents.EndDialog evt)
    {
        if (evt.eventID == eventID)
        {
            EventBus.Instance.Publish<UIEvents.SceneDiscover>(new UIEvents.SceneDiscover());

            Debug.Log($"{evt.eventID} 이벤트 트리거 삭제@");
            DestroyTrigger();

            // 게임모드
            pc.CurMode = preMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(preMode));
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTrigger(other.gameObject);
        }
    }

    protected void DestroyTrigger()
    {
        // 한번 발생 후 파괴
        // 파괴 ? 하는게 좋나
        //Destroy(gameObject);
        EventBus.Instance.Publish<GameEvents.ActiveZone>(new GameEvents.ActiveZone(zoneID, false));
        gameObject.SetActive(false);
    }

    private IEnumerator DelayAfter(float delay) {
        yield return new WaitForSeconds(delay);

        if (initialDialog != null)
        {
            EventBus.Instance.Publish<UIEvents.OpenDialog>(new UIEvents.OpenDialog(eventID, initialDialog));
        }
    }
}
