using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC : MonoBehaviour, IActionNpc
{
    public string npcID;
    public bool isContacted = false;
    private PlayerController pc;
    private Dictionary<string, Dictionary<int, NPCDialogData>> allDialogs;
    private Dictionary<int, NPCDialogData> initialDialog;
    private NPCReDialogData reDialogData;
    private IDatabase database;

    private bool isInDialog = false;
    private bool isDelay = false;       // 대화 이후 Interact Delay 중인지
    private float dialogDelay = 0.5f;   // 원하는 딜레이 시간 (초)
    private float delayTimer = 0f;

    public string GetNpcID() => npcID;

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null) Debug.Log("NPC - Failed to Load PlayerController");
    }

    public void Init(IDatabase db)
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

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Subscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }
    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<UIEvents.EndNpcDialog>(OnEndNpcDialog);
        EventBus.Instance.Unsubscribe<UIEvents.MakeChoice>(OnNpcChoice);
    }

    private void Update()
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

    public void Interact()
    {
        if (isInDialog || isDelay) return; // 대화중이거나, 딜레이 중이면 무시

        if (!isContacted)
        {
            // 첫 대면 : 대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, initialDialog));
            isContacted = true;
        }
        else {
            // 재 대화시 재대화
            isInDialog = true;
            EventBus.Instance.Publish<UIEvents.OpenNpcReDialog>(new UIEvents.OpenNpcReDialog(npcID, reDialogData));
        }
    }

    public void OnNpcChoice(UIEvents.MakeChoice evt) {
        // npc선택지일때만
        if (evt.isNpc) {
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
                    EventBus.Instance.Publish<UIEvents.OpenNpcDialog>(new UIEvents.OpenNpcDialog(npcID, branchDialog));
                }
                else { Debug.Log($"{continueID}에 대한 분기대화 없음"); }
            }
            else {
                // 선택지는 있으나, 그 후 이벤트 없을경우
                EventBus.Instance.Publish<UIEvents.CloseDialog>(new UIEvents.CloseDialog(true));
            }
        }
    }

    private void OnEndNpcDialog(UIEvents.EndNpcDialog evt) {
        if (evt.npcID == npcID) {
            isInDialog = false;

            // 대화 종료 후 다음 Interact 사이 딜레이 시작
            isDelay = true;
            delayTimer = dialogDelay;

            // 게임모드 변경 (NPC와의 대화이후는 반드시 Inspect모드임을 전제)
            pc.CurMode = GameMode.InspectMode;
            EventBus.Instance.Publish<GameEvents.GameModeChange>(new GameEvents.GameModeChange(GameMode.InspectMode));
        }
    }

}
