using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC : MonoBehaviour, IActionNpc
{
    public string npcID;
    public string curNpcEventID = null;
    public bool isContacted = false;
    protected PlayerController pc;
    protected Dictionary<string, Dictionary<int, NPCDialogData>> allDialogs;
    protected Dictionary<int, NPCDialogData> initialDialog;
    protected NPCReDialogData reDialogData;
    protected IDatabase database;

    protected bool isInDialog = false;
    protected bool isDelay = false;       // 대화 이후 Interact Delay 중인지
    protected float dialogDelay = 0.5f;   // 원하는 딜레이 시간 (초)
    protected float delayTimer = 0f;


    public string GetNpcID() => npcID;

    protected virtual void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("NPC - Failed to Load PlayerController");
    }

    public virtual void Init(IDatabase db)
    {
        database = db;
        // 해당 npc의 전체 대화 이벤트 가져오기
        allDialogs = database.GetNpcEvent(npcID);
        if (allDialogs == null) Debug.Log("NPC - Failed to Load NpcDialogs");
        initialDialog = database.GetNpcDialog(npcID, npcID);
        if (initialDialog == null) Debug.Log("NPC - Failed to Load NpcDialogs");
        reDialogData = database.GetNpcReDialog(npcID);
        if (reDialogData == null) Debug.Log("NPC - Failed to Load NpcReDialogs");
    }

    protected void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }
    protected void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }

    protected virtual void Update()
    {
        // 딜레이 타이밍 갱신
        if (isDelay) {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f) {
                isDelay = false;
                delayTimer = 0f;
            }
        }
    }

    public virtual void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (!isContacted)
        {
            // 첫 대면 : 대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, npcID, initialDialog));
            curNpcEventID = npcID;
            isContacted = true;
        }
        else {
            // 재 대화시 재대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    public virtual void OnNpcChoice(UIEvents.MakeChoice evt) {
        // npc선택지일때만
        if (evt.isNpc && evt.data.rootID == npcID) {
            // ChoiceData에서 continue꺼냄
            string continueID = null;
            if (evt.data != null && evt.data.continueIDs.Count > evt.selectIdx)
            {
                continueID = evt.data.continueIDs[evt.selectIdx];
            }

            if (!string.IsNullOrEmpty(continueID))
            {
                if (allDialogs.TryGetValue(continueID, out var branchDialog))
                {
                    // 선택지 이후 분기대화
                    EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, continueID, branchDialog));
                    curNpcEventID = continueID;
                }
                else { Debug.Log($"{continueID}에 대한 분기대화 없음"); }
            }
            else {
                // 선택지는 있으나, 그 후 이벤트 없을경우
                EventBus.Instance.Publish<UIEvents.CloseDialog>(new UIEvents.CloseDialog(true));
            }
        }
    }

    protected virtual void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        //if (evt.npcID == npcID) {
        if (evt.npcID == npcID && evt.npcEventID == curNpcEventID) {
            isInDialog = false;

            // 대화 종료 후 다음 Interact 사이 딜레이 시작
            isDelay = true;
            delayTimer = dialogDelay;

            // 게임모드 변경 (NPC와의 대화이후는 반드시 Inspect모드임을 전제)
            //pc.CurMode = GameMode.InspectMode;
            //EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }

        curNpcEventID = null;       // 대화가끝나면 초기화
    }

}
